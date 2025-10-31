using TicketingBE.Models;

namespace TicketingBE.BLL.Interfaces
{
    /// <summary>
    /// Interface defining business logic operations for TicketType entity
    /// </summary>
    public interface ITicketTypeService
    {
        /// <summary>
        /// Retrieves all ticket types
        /// </summary>
        Task<IEnumerable<TicketType>> GetAllTicketTypesAsync();

        /// <summary>
        /// Retrieves a specific ticket type by its ID
        /// </summary>
        Task<TicketType?> GetTicketTypeByIdAsync(string id);
    }
}
