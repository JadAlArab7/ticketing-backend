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
        Task<User?> GetUserByIdAsync(string id);

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
        Task<string> CreateUserAsync(User user);

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
        Task<bool> DeleteUserAsync(string id);

        /// <summary>
        /// Authenticates a user by username and password
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>The user with department information if authentication successful, null otherwise</returns>
        Task<User?> AuthenticateUserAsync(string username, string password);

        /// <summary>
        /// Retrieves all users belonging to a specific parent department
        /// Used by report-user API
        /// </summary>
        /// <param name="parentDepartmentId">The parent department ID</param>
        /// <returns>List of users in the parent department</returns>
        Task<IEnumerable<User>> GetUsersByParentDepartmentAsync(string parentDepartmentId);

        /// <summary>
        /// Retrieves all users in the same department or child departments
        /// Used by rfi-user API
        /// </summary>
        /// <param name="departmentId">The department ID to filter by</param>
        /// <returns>List of users in same department or child departments</returns>
        Task<IEnumerable<User>> GetUsersByDepartmentOrChildrenAsync(string departmentId);
    }
}
