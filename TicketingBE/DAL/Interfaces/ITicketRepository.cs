using TicketingBE.Models;
using TicketingBE.Models.DTOs;

namespace TicketingBE.DAL.Interfaces
{
    /// <summary>
    /// Repository interface for Ticket entity operations
    /// </summary>
    public interface ITicketRepository
    {
        Task<IEnumerable<TicketListItemDto>> GetAllTicketsAsync(string userId, string? sortBy = null, string? order = null);
        Task<TicketDetailDto?> GetTicketByIdAsync(string id);
        Task<string> CreateTicketAsync(CreateTicketDto ticket, string createdBy);
        Task<bool> UpdateTicketAsync(UpdateTicketDto ticket);
        Task<bool> DeleteTicketAsync(string id);
    }
}
