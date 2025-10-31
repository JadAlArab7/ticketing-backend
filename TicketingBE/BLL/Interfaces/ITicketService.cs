using TicketingBE.Models.DTOs;

namespace TicketingBE.BLL.Interfaces
{
    /// <summary>
    /// Service interface for Ticket business logic
    /// </summary>
    public interface ITicketService
    {
        Task<IEnumerable<TicketListItemDto>> GetAllTicketsAsync(string? sortBy = null, string? order = null);
        Task<TicketDetailDto?> GetTicketByIdAsync(string id);
        Task<string> CreateTicketAsync(CreateTicketDto ticket);
        Task<bool> UpdateTicketAsync(UpdateTicketDto ticket);
        Task<bool> DeleteTicketAsync(string id);
    }
}
