using Microsoft.AspNetCore.Mvc;
using TicketingBE.BLL.Interfaces;
using TicketingBE.Models;

namespace TicketingBE.Controllers
{
    /// <summary>
    /// API Controller for TicketType operations
    /// This layer handles HTTP requests/responses and communicates with the Business Logic Layer
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TicketTypeController : ControllerBase
    {
        private readonly ITicketTypeService _ticketTypeService;
        private readonly ILogger<TicketTypeController> _logger;

        public TicketTypeController(ITicketTypeService ticketTypeService, ILogger<TicketTypeController> logger)
        {
            _ticketTypeService = ticketTypeService;
            _logger = logger;
        }

        /// <summary>
        /// GET: api/TicketType
        /// Retrieves all ticket types
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketType>>> GetAllTicketTypes()
        {
            try
            {
                var ticketTypes = await _ticketTypeService.GetAllTicketTypesAsync();
                return Ok(ticketTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ticket types");
                return StatusCode(500, "An error occurred while retrieving ticket types");
            }
        }

        /// <summary>
        /// GET: api/TicketType/{id}
        /// Retrieves a specific ticket type by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketType>> GetTicketTypeById(string id)
        {
            try
            {
                var ticketType = await _ticketTypeService.GetTicketTypeByIdAsync(id);

                if (ticketType == null)
                    return NotFound($"Ticket type with ID {id} not found");

                return Ok(ticketType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ticket type with ID: {TicketTypeId}", id);
                return StatusCode(500, "An error occurred while retrieving the ticket type");
            }
        }
    }
}
