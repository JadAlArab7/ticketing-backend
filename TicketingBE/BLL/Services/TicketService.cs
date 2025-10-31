using TicketingBE.BLL.Interfaces;
using TicketingBE.DAL.Interfaces;
using TicketingBE.Models.DTOs;

namespace TicketingBE.BLL.Services
{
    /// <summary>
    /// Service implementation for Ticket business logic
    /// </summary>
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TicketService> _logger;

        public TicketService(
            ITicketRepository ticketRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<TicketService> logger)
        {
            _ticketRepository = ticketRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<IEnumerable<TicketListItemDto>> GetAllTicketsAsync(string? sortBy = null, string? order = null)
        {
            try
            {
                // Get department ID from JWT claims
                var departmentId = _httpContextAccessor.HttpContext?.User.FindFirst("departmentId")?.Value;
                
                if (string.IsNullOrWhiteSpace(departmentId))
                {
                    throw new UnauthorizedAccessException("Department ID not found in token");
                }

                return await _ticketRepository.GetAllTicketsAsync(departmentId, sortBy, order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all tickets");
                throw;
            }
        }

        public async Task<TicketDetailDto?> GetTicketByIdAsync(string id)
        {
            try
            {
                return await _ticketRepository.GetTicketByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ticket with ID {TicketId}", id);
                throw;
            }
        }

        public async Task<string> CreateTicketAsync(CreateTicketDto ticket)
        {
            try
            {
                // Get department ID from JWT claims
                var departmentId = _httpContextAccessor.HttpContext?.User.FindFirst("departmentId")?.Value;
                
                if (string.IsNullOrWhiteSpace(departmentId))
                {
                    throw new UnauthorizedAccessException("Department ID not found in token");
                }

                return await _ticketRepository.CreateTicketAsync(ticket, departmentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating ticket");
                throw;
            }
        }

        public async Task<bool> UpdateTicketAsync(UpdateTicketDto ticket)
        {
            try
            {
                return await _ticketRepository.UpdateTicketAsync(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating ticket with ID {TicketId}", ticket.Id);
                throw;
            }
        }

        public async Task<bool> DeleteTicketAsync(string id)
        {
            try
            {
                return await _ticketRepository.DeleteTicketAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting ticket with ID {TicketId}", id);
                throw;
            }
        }
    }
}
