using TicketingBE.DAL.Helpers;
using TicketingBE.DAL.Interfaces;
using TicketingBE.Models;

namespace TicketingBE.DAL.Repositories
{
    /// <summary>
    /// Repository implementation for Ticket File entity operations
    /// </summary>
    public class TicketFileRepository : ITicketFileRepository
    {
        private readonly SqlHelper _sqlHelper;
        private readonly ILogger<TicketFileRepository> _logger;

        public TicketFileRepository(SqlHelper sqlHelper, ILogger<TicketFileRepository> logger)
        {
            _sqlHelper = sqlHelper;
            _logger = logger;
        }

        public async Task<IEnumerable<TicketFile>> GetFilesByTicketIdAsync(string ticketId)
        {
            try
            {
                // Get file metadata without file_data for listing
                string query = @"
                    SELECT 
                        id,
                        file_name,
                        content_type,
                        ticket_id,
                        created_at,
                        created_by
                    FROM tck.ticket_files
                    WHERE ticket_id = @ticket_id
                    ORDER BY created_at DESC";

                var parameters = new[] { _sqlHelper.CreateParam("@ticket_id", Guid.Parse(ticketId)) };

                var files = await _sqlHelper.ExecuteReaderAsync(query, reader => new TicketFile
                {
                    Id = TypeHelper.GetGuidAsString(reader, "id"),
                    FileName = TypeHelper.GetString(reader, "file_name"),
                    ContentType = TypeHelper.GetNullableString(reader, "content_type"),
                    TicketId = TypeHelper.GetGuidAsString(reader, "ticket_id"),
                    CreatedAt = TypeHelper.GetDateTime(reader, "created_at"),
                    CreatedBy = TypeHelper.GetGuidAsString(reader, "created_by"),
                    FileData = Array.Empty<byte>() // Don't load file data for listing
                }, parameters);

                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving files for ticket ID {TicketId}", ticketId);
                throw;
            }
        }

        public async Task<TicketFile?> GetFileByIdAsync(string fileId)
        {
            try
            {
                // Get full file including file_data for download
                string query = @"
                    SELECT 
                        id,
                        file_name,
                        content_type,
                        file_data,
                        ticket_id,
                        created_at,
                        created_by
                    FROM tck.ticket_files
                    WHERE id = @id";

                var parameters = new[] { _sqlHelper.CreateParam("@id", Guid.Parse(fileId)) };

                var files = await _sqlHelper.ExecuteReaderAsync(query, reader => new TicketFile
                {
                    Id = TypeHelper.GetGuidAsString(reader, "id"),
                    FileName = TypeHelper.GetString(reader, "file_name"),
                    ContentType = TypeHelper.GetNullableString(reader, "content_type"),
                    FileData = (byte[])reader["file_data"],
                    TicketId = TypeHelper.GetGuidAsString(reader, "ticket_id"),
                    CreatedAt = TypeHelper.GetDateTime(reader, "created_at"),
                    CreatedBy = TypeHelper.GetGuidAsString(reader, "created_by")
                }, parameters);

                return files.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving file with ID {FileId}", fileId);
                throw;
            }
        }

        public async Task<string> AddFileAsync(string ticketId, string fileName, string? contentType, byte[] fileData, string createdBy)
        {
            try
            {
                string fileId = Guid.NewGuid().ToString();

                string query = @"
                    INSERT INTO tck.ticket_files 
                    (id, file_name, content_type, file_data, ticket_id, created_by, created_at)
                    VALUES 
                    (@id, @file_name, @content_type, @file_data, @ticket_id, @created_by, @created_at)";

                var parameters = new[]
                {
                    _sqlHelper.CreateParam("@id", Guid.Parse(fileId)),
                    _sqlHelper.CreateParam("@file_name", fileName),
                    _sqlHelper.CreateParam("@content_type", (object?)contentType ?? DBNull.Value),
                    _sqlHelper.CreateParam("@file_data", fileData),
                    _sqlHelper.CreateParam("@ticket_id", Guid.Parse(ticketId)),
                    _sqlHelper.CreateParam("@created_by", Guid.Parse(createdBy)),
                    _sqlHelper.CreateParam("@created_at", DateTime.Now)
                };

                await _sqlHelper.ExecuteNonQueryAsync(query, parameters);
                _logger.LogInformation("Successfully added file to ticket ID {TicketId}", ticketId);
                return fileId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding file to ticket ID {TicketId}", ticketId);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileId)
        {
            try
            {
                string query = "DELETE FROM tck.ticket_files WHERE id = @id";
                var parameters = new[] { _sqlHelper.CreateParam("@id", Guid.Parse(fileId)) };

                int rowsAffected = await _sqlHelper.ExecuteNonQueryAsync(query, parameters);
                _logger.LogInformation("Successfully deleted file with ID {FileId}", fileId);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting file with ID {FileId}", fileId);
                throw;
            }
        }

        public async Task<bool> DeleteAllFilesByTicketIdAsync(string ticketId)
        {
            try
            {
                string query = "DELETE FROM tck.ticket_files WHERE ticket_id = @ticket_id";
                var parameters = new[] { _sqlHelper.CreateParam("@ticket_id", Guid.Parse(ticketId)) };

                int rowsAffected = await _sqlHelper.ExecuteNonQueryAsync(query, parameters);
                _logger.LogInformation("Successfully deleted all files for ticket ID {TicketId}", ticketId);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting all files for ticket ID {TicketId}", ticketId);
                throw;
            }
        }
    }
}
