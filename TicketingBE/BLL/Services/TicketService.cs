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

        public async Task<TicketDetailDto> UpdateTicketStatusAsync(string ticketId, string nextStatusId)
        {
            try
            {
                if (!Guid.TryParse(ticketId, out _))
                {
                    throw new ArgumentException("Invalid ticket ID format.", nameof(ticketId));
                }

                if (!Guid.TryParse(nextStatusId, out _))
                {
                    throw new ArgumentException("Invalid next status ID format.", nameof(nextStatusId));
                }

                var departmentId = _httpContextAccessor.HttpContext?.User.FindFirst("departmentId")?.Value;

                if (string.IsNullOrWhiteSpace(departmentId))
                {
                    throw new UnauthorizedAccessException("Department ID not found in token");
                }

                var ticket = await _ticketRepository.GetTicketByIdAsync(ticketId);

                if (ticket == null)
                {
                    throw new KeyNotFoundException($"Ticket with ID {ticketId} not found.");
                }

                var isAssignee = await _ticketRepository.IsDepartmentAuthorizedForTicketAsync(ticketId, departmentId);
                var isCreator = string.Equals(ticket.CreatedBy, departmentId, StringComparison.OrdinalIgnoreCase);

                if (!isAssignee && !isCreator)
                {
                    throw new UnauthorizedAccessException("You are not authorized to update the status of this ticket.");
                }

                if (string.Equals(ticket.TicketStatus, nextStatusId, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Ticket is already in the requested status.");
                }

                var isValidTransition = await _ticketRepository.IsValidStatusTransitionAsync(ticket.TicketTypeId, ticket.TicketStatus, nextStatusId);

                if (!isValidTransition)
                {
                    throw new InvalidOperationException("Invalid status transition for the ticket type.");
                }

                var updateSucceeded = await _ticketRepository.UpdateTicketStatusAsync(ticketId, ticket.TicketStatus, nextStatusId);

                if (!updateSucceeded)
                {
                    throw new InvalidOperationException("Ticket status update failed. The ticket may have been updated by another user.");
                }

                var updatedTicket = await _ticketRepository.GetTicketByIdAsync(ticketId);

                if (updatedTicket == null)
                {
                    throw new InvalidOperationException("Ticket status updated but failed to retrieve updated ticket details.");
                }

                return updatedTicket;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status for ticket with ID {TicketId}", ticketId);
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
