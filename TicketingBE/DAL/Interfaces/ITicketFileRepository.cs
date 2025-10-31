using TicketingBE.Models;

namespace TicketingBE.DAL.Interfaces
{
    /// <summary>
    /// Repository interface for Ticket File entity operations
    /// </summary>
    public interface ITicketFileRepository
    {
        Task<IEnumerable<TicketFile>> GetFilesByTicketIdAsync(string ticketId);
        Task<TicketFile?> GetFileByIdAsync(string fileId);
        Task<string> AddFileAsync(string ticketId, string fileName, string? contentType, byte[] fileData, string createdBy);
        Task<bool> DeleteFileAsync(string fileId);
        Task<bool> DeleteAllFilesByTicketIdAsync(string ticketId);
    }
}
