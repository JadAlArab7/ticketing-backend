using TicketingBE.Models;

namespace TicketingBE.DAL.Interfaces
{
    /// <summary>
    /// Interface for User data access operations
    /// Defines the contract for database operations related to User entity
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves all users from the database
        /// </summary>
        /// <returns>A list of all users</returns>
        Task<IEnumerable<User>> GetAllUsersAsync();

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
        /// Creates a new user in the database
        /// </summary>
        /// <param name="user">The user object to create</param>
        /// <returns>The ID of the newly created user</returns>
        Task<int> CreateUserAsync(User user);

        /// <summary>
        /// Updates an existing user in the database
        /// </summary>
        /// <param name="user">The user object with updated information</param>
        /// <returns>True if the update was successful, false otherwise</returns>
        Task<bool> UpdateUserAsync(User user);

        /// <summary>
        /// Deletes a user from the database
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete</param>
        /// <returns>True if the deletion was successful, false otherwise</returns>
        Task<bool> DeleteUserAsync(int id);
    }
}
