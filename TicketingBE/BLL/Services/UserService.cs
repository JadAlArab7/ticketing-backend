using TicketingBE.BLL.Interfaces;
using TicketingBE.DAL.Interfaces;
using TicketingBE.Models;
using TicketingBE.Models.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TicketingBE.BLL.Services
{
    /// <summary>
    /// Implementation of IUserService containing business logic for User operations
    /// This layer handles validation, business rules, and coordinates between Controller and DAL
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserRepository userRepository, IDepartmentRepository departmentRepository, ILogger<UserService> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Retrieves all users
        /// </summary>
        public async Task<IEnumerable<User>> GetAllActiveUsersAsync()
        {
            try
            {
                var allUsers = await _userRepository.GetAllUsersAsync();
                return allUsers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a specific user by their ID
        /// </summary>
        public async Task<User?> GetUserByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
                {
                    _logger.LogWarning("Invalid user ID: {UserId}", id);
                    return null;
                }

                return await _userRepository.GetUserByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID: {UserId}", id);
                throw;
            }
        }

        /// <summary>
        /// Retrieves a user by their email address
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("Invalid email provided");
                    return null;
                }

                return await _userRepository.GetUserByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with email: {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Creates a new user with validation
        /// Business Rules: Validate required fields, username uniqueness, department assignment
        /// </summary>
        public async Task<string> CreateUserAsync(User user)
        {
            try
            {
                // Business rule: Validate required fields
                if (string.IsNullOrWhiteSpace(user.Username))
                    throw new ArgumentException("Username is required");

                if (string.IsNullOrWhiteSpace(user.Password))
                    throw new ArgumentException("Password is required");

                if (string.IsNullOrWhiteSpace(user.DepartmentId) || !Guid.TryParse(user.DepartmentId, out _))
                    throw new ArgumentException("Valid Department is required");

                // Business rule: Check if username already exists
                var existingUser = await _userRepository.GetUserByEmailAsync(user.Username);
                if (existingUser != null)
                    throw new InvalidOperationException("A user with this username already exists");

                var userId = await _userRepository.CreateUserAsync(user);
                _logger.LogInformation("User created successfully with ID: {UserId}", userId);

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing user with validation
        /// Business Rules: Validate username uniqueness (excluding current user), required fields
        /// </summary>
        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                // Business rule: Validate required fields
                if (string.IsNullOrWhiteSpace(user.Id) || !Guid.TryParse(user.Id, out _))
                    throw new ArgumentException("Invalid user ID");

                if (string.IsNullOrWhiteSpace(user.Username))
                    throw new ArgumentException("Username is required");

                if (string.IsNullOrWhiteSpace(user.Password))
                    throw new ArgumentException("Password is required");

                if (string.IsNullOrWhiteSpace(user.DepartmentId) || !Guid.TryParse(user.DepartmentId, out _))
                    throw new ArgumentException("Valid Department is required");

                // Business rule: Check if username is already taken by another user
                var userWithUsername = await _userRepository.GetUserByEmailAsync(user.Username);
                if (userWithUsername != null && userWithUsername.Id != user.Id)
                    throw new InvalidOperationException("Username is already in use by another user");

                var result = await _userRepository.UpdateUserAsync(user);
                _logger.LogInformation("User updated successfully: {UserId}", user.Id);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", user.Id);
                throw;
            }
        }

        /// <summary>
        /// Deactivates a user (deprecated - included for interface compatibility)
        /// </summary>
        public async Task<bool> DeactivateUserAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
                    throw new ArgumentException("Invalid user ID");

                // Note: Current schema doesn't support soft delete
                _logger.LogWarning("Deactivate user called but not implemented for current schema: {UserId}", id);
                return await Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user with ID: {UserId}", id);
                throw;
            }
        }

        /// <summary>
        /// Permanently deletes a user from the database
        /// Business Rule: This is a hard delete - use with caution
        /// </summary>
        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
                    throw new ArgumentException("Invalid user ID");

                var result = await _userRepository.DeleteUserAsync(id);
                _logger.LogInformation("User permanently deleted: {UserId}", id);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
                throw;
            }
        }

        /// <summary>
        /// Helper method to validate email format
        /// Business validation logic
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token
        /// </summary>
        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.LogWarning("Login attempt with empty username or password");
                    return null;
                }

                // Authenticate user
                var user = await _userRepository.AuthenticateUserAsync(request.Username, request.Password);
                
                if (user == null)
                {
                    _logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
                    return null;
                }

                // Generate JWT token
                var token = await GenerateJwtTokenAsync(user);

                _logger.LogInformation("Successful login for user: {Username}", user.Username);

                // Map to response DTO
                return new LoginResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    DepartmentName = user.DepartmentName,
                    DepartmentType = user.DepartmentType,
                    Token = token
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for username: {Username}", request.Username);
                throw;
            }
        }

        /// <summary>
        /// Generates a JWT token for authenticated user
        /// </summary>
        private async Task<string> GenerateJwtTokenAsync(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Fetch parent department ID from the user's department
            string parentDepartmentId = string.Empty;
            try
            {
                if (Guid.TryParse(user.DepartmentId, out var deptGuid))
                {
                    var department = await _departmentRepository.GetDepartmentByIdAsync(deptGuid);
                    if (department?.ParentDepartmentId.HasValue == true)
                    {
                        parentDepartmentId = department.ParentDepartmentId.Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch parent department for user {UserId}", user.Id);
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim("department", user.DepartmentName),
                new Claim("departmentType", user.DepartmentType),
                new Claim("departmentId", user.DepartmentId),
                new Claim("parentDepartmentId", parentDepartmentId)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Retrieves users in the parent department (report-user API)
        /// Extracts parentDepartmentId from JWT claims
        /// </summary>
        public async Task<IEnumerable<UserDto>> GetReportUsersAsync()
        {
            try
            {
                // Extract parentDepartmentId from JWT claims
                var parentDepartmentId = _httpContextAccessor.HttpContext?.User.FindFirst("parentDepartmentId")?.Value;

                if (string.IsNullOrWhiteSpace(parentDepartmentId))
                {
                    _logger.LogWarning("Parent department ID not found in JWT claims");
                    return Enumerable.Empty<UserDto>();
                }

                // Get users from repository
                var users = await _userRepository.GetUsersByParentDepartmentAsync(parentDepartmentId);

                // Map to DTO
                return users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    DepartmentName = u.DepartmentName,
                    DepartmentId = u.DepartmentId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving report users");
                throw;
            }
        }

        /// <summary>
        /// Retrieves users in same department or child departments (rfi-user API)
        /// Extracts departmentId from JWT claims
        /// </summary>
        public async Task<IEnumerable<UserDto>> GetRfiUsersAsync()
        {
            try
            {
                // Extract departmentId from JWT claims
                var departmentId = _httpContextAccessor.HttpContext?.User.FindFirst("departmentId")?.Value;

                if (string.IsNullOrWhiteSpace(departmentId))
                {
                    _logger.LogWarning("Department ID not found in JWT claims");
                    return Enumerable.Empty<UserDto>();
                }

                // Get users from repository
                var users = await _userRepository.GetUsersByDepartmentOrChildrenAsync(departmentId);

                // Map to DTO
                return users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    DepartmentName = u.DepartmentName,
                    DepartmentId = u.DepartmentId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving RFI users");
                throw;
            }
        }
    }
}
