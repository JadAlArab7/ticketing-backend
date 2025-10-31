using Microsoft.AspNetCore.Mvc;
using TicketingBE.BLL.Interfaces;
using TicketingBE.Models;
using TicketingBE.Models.DTOs;

namespace TicketingBE.Controllers
{
    /// <summary>
    /// API Controller for User operations
    /// This layer handles HTTP requests/responses and communicates with the Business Logic Layer
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// GET: api/User
        /// Retrieves all active users
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllActiveUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, "An error occurred while retrieving users");
            }
        }

        /// <summary>
        /// GET: api/User/{id}
        /// Retrieves a specific user by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                    return NotFound($"User with ID {id} not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID: {UserId}", id);
                return StatusCode(500, "An error occurred while retrieving the user");
            }
        }

        /// <summary>
        /// GET: api/User/email/{email}
        /// Retrieves a user by email address
        /// </summary>
        [HttpGet("email/{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);

                if (user == null)
                    return NotFound($"User with email {email} not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with email: {Email}", email);
                return StatusCode(500, "An error occurred while retrieving the user");
            }
        }

        /// <summary>
        /// POST: api/User
        /// Creates a new user
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            try
            {
                var userId = await _userService.CreateUserAsync(user);
                var createdUser = await _userService.GetUserByIdAsync(userId);

                return CreatedAtAction(nameof(GetUserById), new { id = userId }, createdUser);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating user");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while creating user");
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, "An error occurred while creating the user");
            }
        }

        /// <summary>
        /// PUT: api/User/{id}
        /// Updates an existing user
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(Guid id, [FromBody] User user)
        {
            try
            {
                if (id != user.Id)
                    return BadRequest("User ID mismatch");

                var result = await _userService.UpdateUserAsync(user);

                if (!result)
                    return NotFound($"User with ID {id} not found");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating user");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while updating user");
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", id);
                return StatusCode(500, "An error occurred while updating the user");
            }
        }

        /// <summary>
        /// DELETE: api/User/{id}/deactivate
        /// Soft deletes a user (marks as inactive)
        /// </summary>
        [HttpDelete("{id}/deactivate")]
        public async Task<ActionResult> DeactivateUser(int id)
        {
            try
            {
                var result = await _userService.DeactivateUserAsync(id);

                if (!result)
                    return NotFound($"User with ID {id} not found");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while deactivating user");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while deactivating user");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user with ID: {UserId}", id);
                return StatusCode(500, "An error occurred while deactivating the user");
            }
        }

        /// <summary>
        /// DELETE: api/User/{id}
        /// Permanently deletes a user from the database
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);

                if (!result)
                    return NotFound($"User with ID {id} not found");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while deleting user");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
                return StatusCode(500, "An error occurred while deleting the user");
            }
        }

        /// <summary>
        /// POST: api/User/login
        /// Authenticates a user and returns a JWT token
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var response = await _userService.LoginAsync(request);

                if (response == null)
                    return Unauthorized("Invalid username or password");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, "An error occurred during login");
            }
        }
    }
}
