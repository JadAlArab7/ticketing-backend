using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketingBE.BLL.Interfaces;
using TicketingBE.Models.DTOs;

namespace TicketingBE.Controllers
{
    /// <summary>
    /// Controller for Ticket operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<TicketController> _logger;

        public TicketController(ITicketService ticketService, ILogger<TicketController> logger)
        {
            _ticketService = ticketService;
            _logger = logger;
        }

        /// <summary>
        /// Get all tickets with optional sorting
        /// </summary>
        /// <param name="sortBy">Sort by column: Title, CreatedByUser, Type, Deadline</param>
        /// <param name="order">Sort order: ASC or DESC</param>
        /// <returns>List of tickets</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTickets([FromQuery] string? sortBy = null, [FromQuery] string? order = null)
        {
            try
            {
                var tickets = await _ticketService.GetAllTicketsAsync(sortBy, order);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all tickets");
                return StatusCode(500, new { message = "An error occurred while retrieving tickets" });
            }
        }

        /// <summary>
        /// Get ticket by ID with full details
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <returns>Ticket details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(string id)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByIdAsync(id);
                
                if (ticket == null)
                {
                    return NotFound(new { message = $"Ticket with ID {id} not found" });
                }

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ticket with ID {TicketId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the ticket" });
            }
        }

        /// <summary>
        /// Create a new ticket
        /// </summary>
        /// <param name="ticket">Ticket data</param>
        /// <returns>Created ticket ID</returns>
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto ticket)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var ticketId = await _ticketService.CreateTicketAsync(ticket);
                return CreatedAtAction(nameof(GetTicketById), new { id = ticketId }, new { id = ticketId });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized attempt to create ticket");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating ticket");
                return StatusCode(500, new { message = "An error occurred while creating the ticket" });
            }
        }

        /// <summary>
        /// Update an existing ticket
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <param name="ticket">Updated ticket data</param>
        /// <returns>Success status</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(string id, [FromBody] UpdateTicketDto ticket)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != ticket.Id)
                {
                    return BadRequest(new { message = "ID mismatch" });
                }

                var result = await _ticketService.UpdateTicketAsync(ticket);
                
                if (!result)
                {
                    return NotFound(new { message = $"Ticket with ID {id} not found" });
                }

                return Ok(new { message = "Ticket updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating ticket with ID {TicketId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the ticket" });
            }
        }

        /// <summary>
        /// Delete a ticket
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(string id)
        {
            try
            {
                var result = await _ticketService.DeleteTicketAsync(id);
                
                if (!result)
                {
                    return NotFound(new { message = $"Ticket with ID {id} not found" });
                }

                return Ok(new { message = "Ticket deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting ticket with ID {TicketId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the ticket" });
            }
        }
    }
}
