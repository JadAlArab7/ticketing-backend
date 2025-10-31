using TicketingBE.BLL.Interfaces;
using TicketingBE.DAL.Interfaces;
using TicketingBE.Models;

namespace TicketingBE.BLL.Services
{
    /// <summary>
    /// Implementation of IUserService containing business logic for User operations
    /// This layer handles validation, business rules, and coordinates between Controller and DAL
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all active users
        /// Business Rule: Only return users with IsActive = true
        /// </summary>
        public async Task<IEnumerable<User>> GetAllActiveUsersAsync()
        {
            try
            {
                var allUsers = await _userRepository.GetAllUsersAsync();
                // Business logic: Filter only active users
                return allUsers.Where(u => u.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active users");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a specific user by their ID
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
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
        /// Business Rules: Validate email uniqueness, required fields, and set default values
        /// </summary>
        public async Task<int> CreateUserAsync(User user)
        {
            try
            {
                // Business rule: Validate required fields
                if (string.IsNullOrWhiteSpace(user.Username))
                    throw new ArgumentException("Username is required");

                if (string.IsNullOrWhiteSpace(user.Email))
                    throw new ArgumentException("Email is required");

                if (string.IsNullOrWhiteSpace(user.FullName))
                    throw new ArgumentException("Full name is required");

                // Business rule: Validate email format
                if (!IsValidEmail(user.Email))
                    throw new ArgumentException("Invalid email format");

                // Business rule: Check if email already exists
                var existingUser = await _userRepository.GetUserByEmailAsync(user.Email);
                if (existingUser != null)
                    throw new InvalidOperationException("A user with this email already exists");

                // Business rule: Set default values
                user.CreatedAt = DateTime.UtcNow;
                user.IsActive = true;

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
        /// Business Rules: Validate email uniqueness (excluding current user), required fields
        /// </summary>
        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                // Business rule: Validate required fields
                if (user.Id <= 0)
                    throw new ArgumentException("Invalid user ID");

                if (string.IsNullOrWhiteSpace(user.Username))
                    throw new ArgumentException("Username is required");

                if (string.IsNullOrWhiteSpace(user.Email))
                    throw new ArgumentException("Email is required");

                if (string.IsNullOrWhiteSpace(user.FullName))
                    throw new ArgumentException("Full name is required");

                // Business rule: Validate email format
                if (!IsValidEmail(user.Email))
                    throw new ArgumentException("Invalid email format");

                // Business rule: Check if user exists
                var existingUser = await _userRepository.GetUserByIdAsync(user.Id);
                if (existingUser == null)
                    throw new InvalidOperationException("User not found");

                // Business rule: Check if email is already taken by another user
                var userWithEmail = await _userRepository.GetUserByEmailAsync(user.Email);
                if (userWithEmail != null && userWithEmail.Id != user.Id)
                    throw new InvalidOperationException("Email is already in use by another user");

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
        /// Soft deletes a user (marks as inactive)
        /// Business Rule: Don't permanently delete, just deactivate
        /// </summary>
        public async Task<bool> DeactivateUserAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Invalid user ID");

                var user = await _userRepository.GetUserByIdAsync(id);
                if (user == null)
                    throw new InvalidOperationException("User not found");

                // Business rule: Soft delete - set IsActive to false
                user.IsActive = false;
                var result = await _userRepository.UpdateUserAsync(user);

                _logger.LogInformation("User deactivated successfully: {UserId}", id);
                return result;
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
        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                if (id <= 0)
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
    }
}
