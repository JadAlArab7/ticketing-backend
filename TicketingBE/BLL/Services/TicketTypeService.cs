using TicketingBE.BLL.Interfaces;
using TicketingBE.DAL.Interfaces;
using TicketingBE.Models;

namespace TicketingBE.BLL.Services
{
    /// <summary>
    /// Implementation of ITicketTypeService containing business logic for TicketType operations
    /// This layer handles validation, business rules, and coordinates between Controller and DAL
    /// </summary>
    public class TicketTypeService : ITicketTypeService
    {
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly ILogger<TicketTypeService> _logger;

        public TicketTypeService(ITicketTypeRepository ticketTypeRepository, ILogger<TicketTypeService> logger)
        {
            _ticketTypeRepository = ticketTypeRepository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all ticket types
        /// </summary>
        public async Task<IEnumerable<TicketType>> GetAllTicketTypesAsync()
        {
            try
            {
                return await _ticketTypeRepository.GetAllTicketTypesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ticket types");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a specific ticket type by its ID
        /// </summary>
        public async Task<TicketType?> GetTicketTypeByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
                {
                    _logger.LogWarning("Invalid ticket type ID: {TicketTypeId}", id);
                    return null;
                }

                return await _ticketTypeRepository.GetTicketTypeByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ticket type with ID: {TicketTypeId}", id);
                throw;
            }
        }
    }
}
