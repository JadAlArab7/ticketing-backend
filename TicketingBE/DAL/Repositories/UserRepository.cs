using System.Data;
using TicketingBE.DAL.Helpers;
using TicketingBE.DAL.Interfaces;
using TicketingBE.Models;

namespace TicketingBE.DAL.Repositories
{
    /// <summary>
    /// Implementation of IUserRepository using ADO.NET for database operations
    /// This class is responsible for all database interactions related to User entity
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly SqlHelper _sqlHelper;

        public UserRepository(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));
        }

        /// <summary>
        /// Retrieves all users from the database
        /// </summary>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            string query = "SELECT id, username, email, full_name, created_at, is_active FROM users";

            return await Task.Run(() =>
            {
                return _sqlHelper.ExecuteReader<List<User>>(query, (dataReader) =>
                {
                    var users = new List<User>();
                    while (dataReader.Read())
                    {
                        users.Add(new User
                        {
                            Id = TypeHelper.GetInt32(dataReader["id"]),
                            Username = TypeHelper.GetString(dataReader["username"]),
                            Email = TypeHelper.GetString(dataReader["email"]),
                            FullName = TypeHelper.GetString(dataReader["full_name"]),
                            CreatedAt = TypeHelper.GetDateTime(dataReader["created_at"]),
                            IsActive = TypeHelper.GetBoolean(dataReader["is_active"])
                        });
                    }
                    return users;
                });
            });
        }

        /// <summary>
        /// Retrieves a specific user by their ID
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            string query = "SELECT id, username, email, full_name, created_at, is_active FROM users WHERE id = @id";

            return await Task.Run(() =>
            {
                return _sqlHelper.ExecuteReader<User?>(query, (dataReader) =>
                {
                    User? user = null;
                    if (dataReader.Read())
                    {
                        user = new User
                        {
                            Id = TypeHelper.GetInt32(dataReader["id"]),
                            Username = TypeHelper.GetString(dataReader["username"]),
                            Email = TypeHelper.GetString(dataReader["email"]),
                            FullName = TypeHelper.GetString(dataReader["full_name"]),
                            CreatedAt = TypeHelper.GetDateTime(dataReader["created_at"]),
                            IsActive = TypeHelper.GetBoolean(dataReader["is_active"])
                        };
                    }
                    return user;
                }, _sqlHelper.CreateParam("@id", id));
            });
        }

        /// <summary>
        /// Retrieves a user by their email address
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            string query = "SELECT id, username, email, full_name, created_at, is_active FROM users WHERE email = @email";

            return await Task.Run(() =>
            {
                return _sqlHelper.ExecuteReader<User?>(query, (dataReader) =>
                {
                    User? user = null;
                    if (dataReader.Read())
                    {
                        user = new User
                        {
                            Id = TypeHelper.GetInt32(dataReader["id"]),
                            Username = TypeHelper.GetString(dataReader["username"]),
                            Email = TypeHelper.GetString(dataReader["email"]),
                            FullName = TypeHelper.GetString(dataReader["full_name"]),
                            CreatedAt = TypeHelper.GetDateTime(dataReader["created_at"]),
                            IsActive = TypeHelper.GetBoolean(dataReader["is_active"])
                        };
                    }
                    return user;
                }, _sqlHelper.CreateParam("@email", email));
            });
        }

        /// <summary>
        /// Creates a new user in the database
        /// </summary>
        public async Task<int> CreateUserAsync(User user)
        {
            string query = @"INSERT INTO users (username, email, full_name, created_at, is_active) 
                            VALUES (@username, @email, @full_name, @created_at, @is_active)
                            RETURNING id";

            return await Task.Run(() =>
            {
                return _sqlHelper.ExecuteReader<int>(query, (dataReader) =>
                {
                    int userId = 0;
                    if (dataReader.Read())
                    {
                        userId = TypeHelper.GetInt32(dataReader[0]);
                    }
                    return userId;
                },
                _sqlHelper.CreateParam("@username", user.Username),
                _sqlHelper.CreateParam("@email", user.Email),
                _sqlHelper.CreateParam("@full_name", user.FullName),
                _sqlHelper.CreateParam("@created_at", user.CreatedAt),
                _sqlHelper.CreateParam("@is_active", user.IsActive));
            });
        }

        /// <summary>
        /// Updates an existing user in the database
        /// </summary>
        public async Task<bool> UpdateUserAsync(User user)
        {
            string query = @"UPDATE users 
                            SET username = @username, 
                                email = @email, 
                                full_name = @full_name, 
                                is_active = @is_active 
                            WHERE id = @id";

            return await Task.Run(() =>
            {
                int rowsAffected = _sqlHelper.ExecuteData(query,
                    _sqlHelper.CreateParam("@id", user.Id),
                    _sqlHelper.CreateParam("@username", user.Username),
                    _sqlHelper.CreateParam("@email", user.Email),
                    _sqlHelper.CreateParam("@full_name", user.FullName),
                    _sqlHelper.CreateParam("@is_active", user.IsActive));

                return rowsAffected > 0;
            });
        }

        /// <summary>
        /// Deletes a user from the database
        /// </summary>
        public async Task<bool> DeleteUserAsync(int id)
        {
            string query = "DELETE FROM users WHERE id = @id";

            return await Task.Run(() =>
            {
                int rowsAffected = _sqlHelper.ExecuteData(query,
                    _sqlHelper.CreateParam("@id", id));

                return rowsAffected > 0;
            });
        }
    }
}
