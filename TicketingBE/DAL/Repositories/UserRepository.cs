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
        /// Retrieves all users from the database with their department information
        /// </summary>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            string query = @"
                SELECT 
                    u.id, 
                    u.username, 
                    u.password,
                    u.department_id,
                    d.name as department_name,
                    dt.name as department_type
                FROM tck.users u
                INNER JOIN tck.departments d ON u.department_id = d.id
                INNER JOIN tck.department_types dt ON d.department_type_id = dt.id";

            return await Task.Run(() =>
            {
                return _sqlHelper.ExecuteReader<List<User>>(query, (dataReader) =>
                {
                    var users = new List<User>();
                    while (dataReader.Read())
                    {
                        users.Add(new User
                        {
                            Id = TypeHelper.GetGuid(dataReader["id"]),
                            Username = TypeHelper.GetString(dataReader["username"]),
                            Password = TypeHelper.GetString(dataReader["password"]),
                            DepartmentId = TypeHelper.GetGuid(dataReader["department_id"]),
                            DepartmentName = TypeHelper.GetString(dataReader["department_name"]),
                            DepartmentType = TypeHelper.GetString(dataReader["department_type"])
                        });
                    }
                    return users;
                });
            });
        }

        /// <summary>
        /// Retrieves a specific user by their ID with department information
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            string query = @"
                SELECT 
                    u.id, 
                    u.username, 
                    u.password,
                    u.department_id,
                    d.name as department_name,
                    dt.name as department_type
                FROM tck.users u
                INNER JOIN tck.departments d ON u.department_id = d.id
                INNER JOIN tck.department_types dt ON d.department_type_id = dt.id
                WHERE u.id = @id";

            return await Task.Run(() =>
            {
                return _sqlHelper.ExecuteReader<User?>(query, (dataReader) =>
                {
                    User? user = null;
                    if (dataReader.Read())
                    {
                        user = new User
                        {
                            Id = TypeHelper.GetGuid(dataReader["id"]),
                            Username = TypeHelper.GetString(dataReader["username"]),
                            Password = TypeHelper.GetString(dataReader["password"]),
                            DepartmentId = TypeHelper.GetGuid(dataReader["department_id"]),
                            DepartmentName = TypeHelper.GetString(dataReader["department_name"]),
                            DepartmentType = TypeHelper.GetString(dataReader["department_type"])
                        };
                    }
                    return user;
                }, _sqlHelper.CreateParam("@id", id));
            });
        }

        /// <summary>
        /// Retrieves a user by their username with department information
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            string query = @"
                SELECT 
                    u.id, 
                    u.username, 
                    u.password,
                    u.department_id,
                    d.name as department_name,
                    dt.name as department_type
                FROM tck.users u
                INNER JOIN tck.departments d ON u.department_id = d.id
                INNER JOIN tck.department_types dt ON d.department_type_id = dt.id
                WHERE u.username = @username";

            return await Task.Run(() =>
            {
                return _sqlHelper.ExecuteReader<User?>(query, (dataReader) =>
                {
                    User? user = null;
                    if (dataReader.Read())
                    {
                        user = new User
                        {
                            Id = TypeHelper.GetGuid(dataReader["id"]),
                            Username = TypeHelper.GetString(dataReader["username"]),
                            Password = TypeHelper.GetString(dataReader["password"]),
                            DepartmentId = TypeHelper.GetGuid(dataReader["department_id"]),
                            DepartmentName = TypeHelper.GetString(dataReader["department_name"]),
                            DepartmentType = TypeHelper.GetString(dataReader["department_type"])
                        };
                    }
                    return user;
                }, _sqlHelper.CreateParam("@username", email));
            });
        }

        /// <summary>
        /// Creates a new user in the database
        /// </summary>
        public async Task<int> CreateUserAsync(User user)
        {
            string query = @"INSERT INTO tck.users (username, password, department_id) 
                            VALUES (@username, @password, @department_id)
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
                _sqlHelper.CreateParam("@password", user.Password),
                _sqlHelper.CreateParam("@department_id", user.DepartmentId));
            });
        }

        /// <summary>
        /// Updates an existing user in the database
        /// </summary>
        public async Task<bool> UpdateUserAsync(User user)
        {
            string query = @"UPDATE tck.users 
                            SET username = @username, 
                                password = @password, 
                                department_id = @department_id 
                            WHERE id = @id";

            return await Task.Run(() =>
            {
                int rowsAffected = _sqlHelper.ExecuteData(query,
                    _sqlHelper.CreateParam("@id", user.Id),
                    _sqlHelper.CreateParam("@username", user.Username),
                    _sqlHelper.CreateParam("@password", user.Password),
                    _sqlHelper.CreateParam("@department_id", user.DepartmentId));

                return rowsAffected > 0;
            });
        }

        /// <summary>
        /// Deletes a user from the database
        /// </summary>
        public async Task<bool> DeleteUserAsync(int id)
        {
            string query = "DELETE FROM tck.users WHERE id = @id";

            return await Task.Run(() =>
            {
                int rowsAffected = _sqlHelper.ExecuteData(query,
                    _sqlHelper.CreateParam("@id", id));

                return rowsAffected > 0;
            });
        }

        /// <summary>
        /// Authenticates a user by username and password
        /// Joins users, departments, and department_types tables to get complete user information
        /// </summary>
        public async Task<User?> AuthenticateUserAsync(string username, string password)
        {
            string query = @"
                SELECT 
                    u.id, 
                    u.username, 
                    u.password,
                    u.department_id,
                    d.name as department_name,
                    dt.name as department_type
                FROM tck.users u
                INNER JOIN tck.departments d ON u.department_id = d.id
                INNER JOIN tck.department_types dt ON d.department_type_id = dt.id
                WHERE u.username = @username AND u.password = @password";

            return await Task.Run(() =>
            {
                return _sqlHelper.ExecuteReader<User?>(query, (dataReader) =>
                {
                    User? user = null;
                    if (dataReader.Read())
                    {
                        user = new User
                        {
                            Id = TypeHelper.GetGuid(dataReader["id"]),
                            Username = TypeHelper.GetString(dataReader["username"]),
                            Password = TypeHelper.GetString(dataReader["password"]),
                            DepartmentId = TypeHelper.GetGuid(dataReader["department_id"]),
                            DepartmentName = TypeHelper.GetString(dataReader["department_name"]),
                            DepartmentType = TypeHelper.GetString(dataReader["department_type"])
                        };
                    }
                    return user;
                },
                _sqlHelper.CreateParam("@username", username),
                _sqlHelper.CreateParam("@password", password));
            });
        }
    }
}
