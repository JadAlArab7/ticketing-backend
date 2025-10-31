using TicketingBE.DAL.Helpers;
using TicketingBE.DAL.Interfaces;
using TicketingBE.Models;

namespace TicketingBE.DAL.Repositories
{
    /// <summary>
    /// Repository implementation for Ticket Assignee entity operations
    /// </summary>
    public class TicketAssigneeRepository : ITicketAssigneeRepository
    {
        private readonly SqlHelper _sqlHelper;
        private readonly ILogger<TicketAssigneeRepository> _logger;

        public TicketAssigneeRepository(SqlHelper sqlHelper, ILogger<TicketAssigneeRepository> logger)
        {
            _sqlHelper = sqlHelper;
            _logger = logger;
        }

        public async Task<IEnumerable<TicketAssignee>> GetAssigneesByTicketIdAsync(string ticketId)
        {
            try
            {
                string query = @"
                    SELECT 
                        ta.ticket_id,
                        ta.department_id,
                        d.name AS department_name,
                        ta.ticket_assignee_type,
                        tat.name AS ticket_assignee_type_name
                    FROM tck.ticket_assignees ta
                    INNER JOIN tck.departments d ON ta.department_id = d.id
                    LEFT JOIN tck.ticket_assignee_types tat ON ta.ticket_assignee_type = tat.id
                    WHERE ta.ticket_id = @ticket_id";

                var parameters = new[] { _sqlHelper.CreateParam("@ticket_id", Guid.Parse(ticketId)) };

                var assignees = await _sqlHelper.ExecuteReaderAsync(query, reader => new TicketAssignee
                {
                    TicketId = TypeHelper.GetGuidAsString(reader, "ticket_id"),
                    DepartmentId = TypeHelper.GetGuidAsString(reader, "department_id"),
                    DepartmentName = TypeHelper.GetString(reader, "department_name"),
                    TicketAssigneeType = TypeHelper.GetGuidAsString(reader, "ticket_assignee_type"),
                    TicketAssigneeTypeName = TypeHelper.GetString(reader, "ticket_assignee_type_name") ?? string.Empty
                }, parameters);

                return assignees;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving assignees for ticket ID {TicketId}", ticketId);
                throw;
            }
        }

        public async Task<bool> AddAssigneeAsync(string ticketId, string departmentId, string ticketAssigneeType)
        {
            try
            {
                string query = @"
                    INSERT INTO tck.ticket_assignees (ticket_id, department_id, ticket_assignee_type)
                    VALUES (@ticket_id, @department_id, @ticket_assignee_type)
                    ON CONFLICT (ticket_id, department_id) DO UPDATE 
                    SET ticket_assignee_type = @ticket_assignee_type";

                var parameters = new[]
                {
                    _sqlHelper.CreateParam("@ticket_id", Guid.Parse(ticketId)),
                    _sqlHelper.CreateParam("@department_id", Guid.Parse(departmentId)),
                    _sqlHelper.CreateParam("@ticket_assignee_type", Guid.Parse(ticketAssigneeType))
                };

                int rowsAffected = await _sqlHelper.ExecuteNonQueryAsync(query, parameters);
                _logger.LogInformation("Successfully added assignee to ticket ID {TicketId}", ticketId);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding assignee to ticket ID {TicketId}", ticketId);
                throw;
            }
        }

        public async Task<bool> RemoveAssigneeAsync(string ticketId, string departmentId)
        {
            try
            {
                string query = @"
                    DELETE FROM tck.ticket_assignees 
                    WHERE ticket_id = @ticket_id AND department_id = @department_id";

                var parameters = new[]
                {
                    _sqlHelper.CreateParam("@ticket_id", Guid.Parse(ticketId)),
                    _sqlHelper.CreateParam("@department_id", Guid.Parse(departmentId))
                };

                int rowsAffected = await _sqlHelper.ExecuteNonQueryAsync(query, parameters);
                _logger.LogInformation("Successfully removed assignee from ticket ID {TicketId}", ticketId);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing assignee from ticket ID {TicketId}", ticketId);
                throw;
            }
        }

        public async Task<bool> RemoveAllAssigneesByTicketIdAsync(string ticketId)
        {
            try
            {
                string query = "DELETE FROM tck.ticket_assignees WHERE ticket_id = @ticket_id";
                var parameters = new[] { _sqlHelper.CreateParam("@ticket_id", Guid.Parse(ticketId)) };

                int rowsAffected = await _sqlHelper.ExecuteNonQueryAsync(query, parameters);
                _logger.LogInformation("Successfully removed all assignees from ticket ID {TicketId}", ticketId);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing all assignees from ticket ID {TicketId}", ticketId);
                throw;
            }
        }
    }
}
