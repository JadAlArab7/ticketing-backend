using TicketingBE.Models;

namespace TicketingBE.DAL.Interfaces
{
    /// <summary>
    /// Interface defining data access operations for TicketType entity
    /// </summary>
    public interface ITicketTypeRepository
    {
        /// <summary>
        /// Retrieves all ticket types from the database
        /// </summary>
        Task<IEnumerable<TicketType>> GetAllTicketTypesAsync();

        /// <summary>
        /// Retrieves a specific ticket type by its ID
        /// </summary>
        Task<TicketType?> GetTicketTypeByIdAsync(string id);
    }
}
