using TicketingBE.Models;

namespace TicketingBE.BLL.Interfaces
{
    /// <summary>
    /// Interface for User business logic operations
    /// Defines the contract for business operations related to User entity
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Retrieves all active users
        /// </summary>
        /// <returns>A list of all active users</returns>
        Task<IEnumerable<User>> GetAllActiveUsersAsync();

        /// <summary>
        /// Retrieves a specific user by their ID
        /// </summary>
        /// <param name="id">The unique identifier of the user</param>
        /// <returns>The user if found, null otherwise</returns>
        Task<User?> GetUserByIdAsync(int id);

        /// <summary>
        /// Retrieves a user by their email address
        /// </summary>
        /// <param name="email">The email address to search for</param>
        /// <returns>The user if found, null otherwise</returns>
        Task<User?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Creates a new user with validation
        /// </summary>
        /// <param name="user">The user object to create</param>
        /// <returns>The ID of the newly created user</returns>
        Task<int> CreateUserAsync(User user);

        /// <summary>
        /// Updates an existing user with validation
        /// </summary>
        /// <param name="user">The user object with updated information</param>
        /// <returns>True if the update was successful, false otherwise</returns>
        Task<bool> UpdateUserAsync(User user);

        /// <summary>
        /// Soft deletes a user (marks as inactive)
        /// </summary>
        /// <param name="id">The unique identifier of the user to deactivate</param>
        /// <returns>True if the deactivation was successful, false otherwise</returns>
        Task<bool> DeactivateUserAsync(int id);

        /// <summary>
        /// Permanently deletes a user from the database
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete</param>
        /// <returns>True if the deletion was successful, false otherwise</returns>
        Task<bool> DeleteUserAsync(int id);
    }
}
