using Npgsql;
using TicketingBE.DAL.Helpers;
using TicketingBE.DAL.Interfaces;
using TicketingBE.Models;
using TicketingBE.Models.DTOs;

namespace TicketingBE.DAL.Repositories
{
    /// <summary>
    /// Repository implementation for Ticket entity operations
    /// </summary>
    public class TicketRepository : ITicketRepository
    {
        private readonly SqlHelper _sqlHelper;
        private readonly ITicketAssigneeRepository _ticketAssigneeRepository;
        private readonly ITicketFileRepository _ticketFileRepository;
        private readonly ILogger<TicketRepository> _logger;

        public TicketRepository(
            SqlHelper sqlHelper,
            ITicketAssigneeRepository ticketAssigneeRepository,
            ITicketFileRepository ticketFileRepository,
            ILogger<TicketRepository> logger)
        {
            _sqlHelper = sqlHelper;
            _ticketAssigneeRepository = ticketAssigneeRepository;
            _ticketFileRepository = ticketFileRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<TicketListItemDto>> GetAllTicketsAsync(string userId, string? sortBy = null, string? order = null)
        {
            try
            {
                // Build ORDER BY clause dynamically
                string orderByClause = BuildOrderByClause(sortBy, order);

                // Filter tickets: created by user OR assigned to user's department
                string query = $@"
                    SELECT DISTINCT
                        t.id,
                        t.subject,
                        tt.name AS ticket_type_name,
                        ts.name AS ticket_status_name,
                        d.name AS created_by_department_name,
                        t.deadline,
                        t.created_at
                    FROM tck.tickets t
                    INNER JOIN tck.ticket_types tt ON t.ticket_type_id = tt.id
                    LEFT JOIN tck.ticket_status ts ON t.ticket_status = ts.id
                    INNER JOIN tck.departments d ON t.created_by = d.id
                    LEFT JOIN tck.ticket_assignees ta ON t.id = ta.ticket_id
                    WHERE t.created_by = @user_id OR ta.department_id = @user_id
                    {orderByClause}";

                var parameters = new[] { _sqlHelper.CreateParam("@user_id", Guid.Parse(userId)) };

                var tickets = await _sqlHelper.ExecuteReaderAsync(query, reader => new TicketListItemDto
                {
                    Id = TypeHelper.GetGuidAsString(reader, "id"),
                    Subject = TypeHelper.GetString(reader, "subject"),
                    TicketTypeName = TypeHelper.GetString(reader, "ticket_type_name"),
                    TicketStatusName = TypeHelper.GetString(reader, "ticket_status_name") ?? string.Empty,
                    CreatedByDepartmentName = TypeHelper.GetString(reader, "created_by_department_name"),
                    Deadline = TypeHelper.GetNullableDateTime(reader, "deadline"),
                    CreatedAt = TypeHelper.GetDateTime(reader, "created_at")
                }, parameters);

                // Load assignees for each ticket
                foreach (var ticket in tickets)
                {
                    var assignees = await _ticketAssigneeRepository.GetAssigneesByTicketIdAsync(ticket.Id);
                    ticket.AssignedToDepartments = assignees.Select(a => a.DepartmentName).ToList();
                }

                return tickets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all tickets for user {UserId}", userId);
                throw;
            }
        }

        public async Task<TicketDetailDto?> GetTicketByIdAsync(string id)
        {
            try
            {
                string query = @"
                    SELECT 
                        t.id,
                        t.ticket_type_id,
                        tt.name AS ticket_type_name,
                        t.subject,
                        t.description,
                        t.alert_buffer,
                        t.deadline,
                        t.created_at,
                        t.ticket_status,
                        ts.name AS ticket_status_name,
                        t.created_by,
                        d.name AS created_by_department_name
                    FROM tck.tickets t
                    INNER JOIN tck.ticket_types tt ON t.ticket_type_id = tt.id
                    LEFT JOIN tck.ticket_status ts ON t.ticket_status = ts.id
                    INNER JOIN tck.departments d ON t.created_by = d.id
                    WHERE t.id = @id";

                var parameters = new[] { _sqlHelper.CreateParam("@id", Guid.Parse(id)) };

                var tickets = await _sqlHelper.ExecuteReaderAsync(query, reader => new TicketDetailDto
                {
                    Id = TypeHelper.GetGuidAsString(reader, "id"),
                    TicketTypeId = TypeHelper.GetGuidAsString(reader, "ticket_type_id"),
                    TicketTypeName = TypeHelper.GetString(reader, "ticket_type_name"),
                    Subject = TypeHelper.GetString(reader, "subject"),
                    Description = TypeHelper.GetString(reader, "description"),
                    AlertBuffer = TypeHelper.GetNullableDateTime(reader, "alert_buffer"),
                    Deadline = TypeHelper.GetNullableDateTime(reader, "deadline"),
                    CreatedAt = TypeHelper.GetDateTime(reader, "created_at"),
                    TicketStatus = TypeHelper.GetGuidAsString(reader, "ticket_status"),
                    TicketStatusName = TypeHelper.GetString(reader, "ticket_status_name") ?? string.Empty,
                    CreatedBy = TypeHelper.GetGuidAsString(reader, "created_by"),
                    CreatedByDepartmentName = TypeHelper.GetString(reader, "created_by_department_name")
                }, parameters);

                var ticket = tickets.FirstOrDefault();
                if (ticket == null)
                    return null;

                // Load assignees
                var assignees = await _ticketAssigneeRepository.GetAssigneesByTicketIdAsync(ticket.Id);
                ticket.Assignees = assignees.Select(a => new TicketAssigneeDto
                {
                    TicketId = a.TicketId,
                    DepartmentId = a.DepartmentId,
                    DepartmentName = a.DepartmentName,
                    TicketAssigneeType = a.TicketAssigneeType,
                    TicketAssigneeTypeName = a.TicketAssigneeTypeName
                }).ToList();

                // Load files (without file data)
                var files = await _ticketFileRepository.GetFilesByTicketIdAsync(ticket.Id);
                ticket.Files = files.Select(f => new TicketFileDto
                {
                    Id = f.Id,
                    FileName = f.FileName,
                    ContentType = f.ContentType,
                    TicketId = f.TicketId,
                    CreatedAt = f.CreatedAt,
                    CreatedBy = f.CreatedBy
                }).ToList();

                return ticket;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ticket with ID {TicketId}", id);
                throw;
            }
        }

        public async Task<string> CreateTicketAsync(CreateTicketDto ticket, string createdBy)
        {
            NpgsqlConnection? connection = null;
            NpgsqlTransaction? transaction = null;

            try
            {
                connection = _sqlHelper.GetConnection();
                await connection.OpenAsync();
                transaction = await connection.BeginTransactionAsync();

                // Generate new ticket ID
                string ticketId = Guid.NewGuid().ToString();

                // Default status: Draft (9921a92c-c0f7-4c14-95c5-3774e5c3d81b)
                string defaultDraftStatusId = "9921a92c-c0f7-4c14-95c5-3774e5c3d81b";

                // Insert ticket
                string insertQuery = @"
                    INSERT INTO tck.tickets 
                    (id, ticket_type_id, subject, description, alert_buffer, deadline, ticket_status, created_by, created_at)
                    VALUES 
                    (@id, @ticket_type_id, @subject, @description, @alert_buffer, @deadline, @ticket_status, @created_by, @created_at)";

                var parameters = new[]
                {
                    _sqlHelper.CreateParam("@id", Guid.Parse(ticketId)),
                    _sqlHelper.CreateParam("@ticket_type_id", Guid.Parse(ticket.TicketTypeId)),
                    _sqlHelper.CreateParam("@subject", ticket.Subject),
                    _sqlHelper.CreateParam("@description", ticket.Description),
                    _sqlHelper.CreateParam("@alert_buffer", ticket.AlertBuffer.HasValue ? DateTime.SpecifyKind(ticket.AlertBuffer.Value, DateTimeKind.Unspecified) : DBNull.Value),
                    _sqlHelper.CreateParam("@deadline", ticket.Deadline.HasValue ? DateTime.SpecifyKind(ticket.Deadline.Value, DateTimeKind.Unspecified) : DBNull.Value),
                    _sqlHelper.CreateParam("@ticket_status", Guid.Parse(defaultDraftStatusId)),
                    _sqlHelper.CreateParam("@created_by", Guid.Parse(createdBy)),
                    _sqlHelper.CreateParam("@created_at", DateTime.Now)
                };

                await _sqlHelper.ExecuteNonQueryAsync(insertQuery, parameters, transaction);

                // Insert assignee if provided
                if (!string.IsNullOrWhiteSpace(ticket.AssigneeDepartmentId))
                {
                    string defaultAssigneeTypeId = "e58efe89-2c1d-4102-b328-25ecdfb600dd";
                    await InsertAssigneeAsync(ticketId, ticket.AssigneeDepartmentId, defaultAssigneeTypeId, transaction);
                }

                // Insert files
                foreach (var file in ticket.Files)
                {
                    await InsertFileAsync(ticketId, file.FileName, file.ContentType, file.FileData, createdBy, transaction);
                }

                await transaction.CommitAsync();
                _logger.LogInformation("Successfully created ticket with ID {TicketId}", ticketId);
                return ticketId;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    await transaction.RollbackAsync();
                
                _logger.LogError(ex, "Error occurred while creating ticket");
                throw;
            }
            finally
            {
                if (transaction != null)
                    await transaction.DisposeAsync();
                if (connection != null)
                    await connection.DisposeAsync();
            }
        }

        public async Task<bool> UpdateTicketAsync(UpdateTicketDto ticket)
        {
            NpgsqlConnection? connection = null;
            NpgsqlTransaction? transaction = null;

            try
            {
                connection = _sqlHelper.GetConnection();
                await connection.OpenAsync();
                transaction = await connection.BeginTransactionAsync();

                // Update ticket
                string updateQuery = @"
                    UPDATE tck.tickets 
                    SET 
                        ticket_type_id = @ticket_type_id,
                        subject = @subject,
                        description = @description,
                        alert_buffer = @alert_buffer,
                        deadline = @deadline
                    WHERE id = @id";

                var parameters = new[]
                {
                    _sqlHelper.CreateParam("@id", Guid.Parse(ticket.Id)),
                    _sqlHelper.CreateParam("@ticket_type_id", Guid.Parse(ticket.TicketTypeId)),
                    _sqlHelper.CreateParam("@subject", ticket.Subject),
                    _sqlHelper.CreateParam("@description", ticket.Description),
                    _sqlHelper.CreateParam("@alert_buffer", ticket.AlertBuffer.HasValue ? DateTime.SpecifyKind(ticket.AlertBuffer.Value, DateTimeKind.Unspecified) : DBNull.Value),
                    _sqlHelper.CreateParam("@deadline", ticket.Deadline.HasValue ? DateTime.SpecifyKind(ticket.Deadline.Value, DateTimeKind.Unspecified) : DBNull.Value)
                   // _sqlHelper.CreateParam("@ticket_status", Guid.Parse(ticket.TicketStatus))
                };

                int rowsAffected = await _sqlHelper.ExecuteNonQueryAsync(updateQuery, parameters, transaction);

                if (rowsAffected == 0)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                // Remove all existing assignees
                await RemoveAllAssigneesAsync(ticket.Id, transaction);
                
                // Insert new assignee if provided
                if (!string.IsNullOrWhiteSpace(ticket.AssigneeDepartmentId))
                {
                    string defaultAssigneeTypeId = "e58efe89-2c1d-4102-b328-25ecdfb600dd";
                    await InsertAssigneeAsync(ticket.Id, ticket.AssigneeDepartmentId, defaultAssigneeTypeId, transaction);
                }

                await transaction.CommitAsync();
                _logger.LogInformation("Successfully updated ticket with ID {TicketId}", ticket.Id);
                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    await transaction.RollbackAsync();
                
                _logger.LogError(ex, "Error occurred while updating ticket with ID {TicketId}", ticket.Id);
                throw;
            }
            finally
            {
                if (transaction != null)
                    await transaction.DisposeAsync();
                if (connection != null)
                    await connection.DisposeAsync();
            }
        }

        public async Task<bool> DeleteTicketAsync(string id)
        {
            NpgsqlConnection? connection = null;
            NpgsqlTransaction? transaction = null;

            try
            {
                connection = _sqlHelper.GetConnection();
                await connection.OpenAsync();
                transaction = await connection.BeginTransactionAsync();

                // Delete assignees
                await RemoveAllAssigneesAsync(id, transaction);

                // Delete files
                await DeleteAllFilesAsync(id, transaction);

                // Delete ticket
                string deleteQuery = "DELETE FROM tck.tickets WHERE id = @id";
                var parameters = new[] { _sqlHelper.CreateParam("@id", Guid.Parse(id)) };
                int rowsAffected = await _sqlHelper.ExecuteNonQueryAsync(deleteQuery, parameters, transaction);

                await transaction.CommitAsync();
                _logger.LogInformation("Successfully deleted ticket with ID {TicketId}", id);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    await transaction.RollbackAsync();
                
                _logger.LogError(ex, "Error occurred while deleting ticket with ID {TicketId}", id);
                throw;
            }
            finally
            {
                if (transaction != null)
                    await transaction.DisposeAsync();
                if (connection != null)
                    await connection.DisposeAsync();
            }
        }

        // Private helper methods

        private string BuildOrderByClause(string? sortBy, string? order)
        {
            // Default sorting
            string orderByClause = "ORDER BY t.created_at DESC";

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                string orderDirection = order?.ToUpper() == "ASC" ? "ASC" : "DESC";

                orderByClause = sortBy.ToLower() switch
                {
                    "title" or "subject" => $"ORDER BY t.subject {orderDirection}",
                    "createdbyuser" or "createdby" => $"ORDER BY d.name {orderDirection}",
                    "type" or "tickettype" => $"ORDER BY tt.name {orderDirection}",
                    "deadline" => $"ORDER BY t.deadline {orderDirection}",
                    "createdat" => $"ORDER BY t.created_at {orderDirection}",
                    _ => orderByClause
                };
            }

            return orderByClause;
        }

        private async Task InsertAssigneeAsync(string ticketId, string departmentId, string ticketAssigneeType, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO tck.ticket_assignees (ticket_id, department_id, ticket_assignee_type)
                VALUES (@ticket_id, @department_id, @ticket_assignee_type)";

            var parameters = new[]
            {
                _sqlHelper.CreateParam("@ticket_id", Guid.Parse(ticketId)),
                _sqlHelper.CreateParam("@department_id", Guid.Parse(departmentId)),
                _sqlHelper.CreateParam("@ticket_assignee_type", Guid.Parse(ticketAssigneeType))
            };

            await _sqlHelper.ExecuteNonQueryAsync(query, parameters, transaction);
        }

        private async Task RemoveAllAssigneesAsync(string ticketId, NpgsqlTransaction transaction)
        {
            string query = "DELETE FROM tck.ticket_assignees WHERE ticket_id = @ticket_id";
            var parameters = new[] { _sqlHelper.CreateParam("@ticket_id", Guid.Parse(ticketId)) };
            await _sqlHelper.ExecuteNonQueryAsync(query, parameters, transaction);
        }

        private async Task InsertFileAsync(string ticketId, string fileName, string? contentType, byte[] fileData, string createdBy, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO tck.ticket_files (id, file_name, content_type, file_data, ticket_id, created_by, created_at)
                VALUES (@id, @file_name, @content_type, @file_data, @ticket_id, @created_by, @created_at)";

            var parameters = new[]
            {
                _sqlHelper.CreateParam("@id", Guid.NewGuid()),
                _sqlHelper.CreateParam("@file_name", fileName),
                _sqlHelper.CreateParam("@content_type", (object?)contentType ?? DBNull.Value),
                _sqlHelper.CreateParam("@file_data", fileData),
                _sqlHelper.CreateParam("@ticket_id", Guid.Parse(ticketId)),
                _sqlHelper.CreateParam("@created_by", Guid.Parse(createdBy)),
                _sqlHelper.CreateParam("@created_at", DateTime.Now)
            };

            await _sqlHelper.ExecuteNonQueryAsync(query, parameters, transaction);
        }

        private async Task DeleteAllFilesAsync(string ticketId, NpgsqlTransaction transaction)
        {
            string query = "DELETE FROM tck.ticket_files WHERE ticket_id = @ticket_id";
            var parameters = new[] { _sqlHelper.CreateParam("@ticket_id", Guid.Parse(ticketId)) };
            await _sqlHelper.ExecuteNonQueryAsync(query, parameters, transaction);
        }
    }
}
