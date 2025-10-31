using System.Data;
using TicketingBE.DAL.Helpers;
using TicketingBE.DAL.Interfaces;
using TicketingBE.Models;
using TicketingBE.Models.DTOs;

namespace TicketingBE.DAL.Repositories
{
    /// <summary>
    /// Implementation of IDepartmentRepository using ADO.NET for database operations
    /// This class is responsible for all database interactions related to Department entity
    /// </summary>
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly SqlHelper _sqlHelper;

        public DepartmentRepository(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));
        }

        /// <summary>
        /// Retrieves all departments from the database with their type and parent information
        /// </summary>
        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            string query = @"
                SELECT 
                    d.id, 
                    d.name,
                    d.department_type_id,
                    dt.name as department_type_name,
                    d.parent_department_id,
                    pd.name as parent_department_name
                FROM tck.departments d
                INNER JOIN tck.department_types dt ON d.department_type_id = dt.id
                LEFT JOIN tck.departments pd ON d.parent_department_id = pd.id
                ORDER BY d.name";

            return await Task.Run(() =>
            {
                return _sqlHelper.ExecuteReader<List<Department>>(query, (dataReader) =>
                {
                    var departments = new List<Department>();
                    while (dataReader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = TypeHelper.GetGuid(dataReader["id"]),
                            Name = TypeHelper.GetString(dataReader["name"]),
                            DepartmentTypeId = TypeHelper.GetGuid(dataReader["department_type_id"]),
                            DepartmentTypeName = TypeHelper.GetString(dataReader["department_type_name"]),
                            ParentDepartmentId = TypeHelper.GetNullableGuid(dataReader["parent_department_id"]),
                            ParentDepartmentName = TypeHelper.GetNullableString(dataReader["parent_department_name"])
                        });
                    }
                    return departments;
                });
            });
        }

        /// <summary>
        /// Retrieves a specific department by its ID with type and parent information
        /// </summary>
        public async Task<Department?> GetDepartmentByIdAsync(Guid id)
        {
            string query = @"
                SELECT 
                    d.id, 
                    d.name,
                    d.department_type_id,
                    dt.name as department_type_name,
                    d.parent_department_id,
                    pd.name as parent_department_name
                FROM tck.departments d
                INNER JOIN tck.department_types dt ON d.department_type_id = dt.id
                LEFT JOIN tck.departments pd ON d.parent_department_id = pd.id
                WHERE d.id = @id";

            return await Task.Run(() =>
            {
                return _sqlHelper.ExecuteReader<Department?>(query, (dataReader) =>
                {
                    Department? department = null;
                    if (dataReader.Read())
                    {
                        department = new Department
                        {
                            Id = TypeHelper.GetGuid(dataReader["id"]),
                            Name = TypeHelper.GetString(dataReader["name"]),
                            DepartmentTypeId = TypeHelper.GetGuid(dataReader["department_type_id"]),
                            DepartmentTypeName = TypeHelper.GetString(dataReader["department_type_name"]),
                            ParentDepartmentId = TypeHelper.GetNullableGuid(dataReader["parent_department_id"]),
                            ParentDepartmentName = TypeHelper.GetNullableString(dataReader["parent_department_name"])
                        };
                    }
                    return department;
                }, _sqlHelper.CreateParam("@id", id));
            });
        }

        /// <summary>
        /// Retrieves all departments by department type
        /// </summary>
        public async Task<IEnumerable<Department>> GetDepartmentsByTypeAsync(Guid departmentTypeId)
        {
            string query = @"
                SELECT 
                    d.id, 
                    d.name,
                    d.department_type_id,
                    dt.name as department_type_name,
                    d.parent_department_id,
                    pd.name as parent_department_name
                FROM tck.departments d
                INNER JOIN tck.department_types dt ON d.department_type_id = dt.id
                LEFT JOIN tck.departments pd ON d.parent_department_id = pd.id
                WHERE d.department_type_id = @departmentTypeId
                ORDER BY d.name";

            return await Task.Run(() =>
            {
                return _sqlHelper.ExecuteReader<List<Department>>(query, (dataReader) =>
                {
                    var departments = new List<Department>();
                    while (dataReader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = TypeHelper.GetGuid(dataReader["id"]),
                            Name = TypeHelper.GetString(dataReader["name"]),
                            DepartmentTypeId = TypeHelper.GetGuid(dataReader["department_type_id"]),
                            DepartmentTypeName = TypeHelper.GetString(dataReader["department_type_name"]),
                            ParentDepartmentId = TypeHelper.GetNullableGuid(dataReader["parent_department_id"]),
                            ParentDepartmentName = TypeHelper.GetNullableString(dataReader["parent_department_name"])
                        });
                    }
                    return departments;
                }, _sqlHelper.CreateParam("@departmentTypeId", departmentTypeId));
            });
        }

        /// <summary>
        /// Retrieves all child departments of a parent department
        /// </summary>
        public async Task<IEnumerable<Department>> GetChildDepartmentsAsync(Guid parentDepartmentId)
        {
            string query = @"
                SELECT 
                    d.id, 
                    d.name,
                    d.department_type_id,
                    dt.name as department_type_name,
                    d.parent_department_id,
                    pd.name as parent_department_name
                FROM tck.departments d
                INNER JOIN tck.department_types dt ON d.department_type_id = dt.id
                LEFT JOIN tck.departments pd ON d.parent_department_id = pd.id
                WHERE d.parent_department_id = @parentDepartmentId
                ORDER BY d.name";

            return await Task.Run(() =>
            {
                return _sqlHelper.ExecuteReader<List<Department>>(query, (dataReader) =>
                {
                    var departments = new List<Department>();
                    while (dataReader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = TypeHelper.GetGuid(dataReader["id"]),
                            Name = TypeHelper.GetString(dataReader["name"]),
                            DepartmentTypeId = TypeHelper.GetGuid(dataReader["department_type_id"]),
                            DepartmentTypeName = TypeHelper.GetString(dataReader["department_type_name"]),
                            ParentDepartmentId = TypeHelper.GetNullableGuid(dataReader["parent_department_id"]),
                            ParentDepartmentName = TypeHelper.GetNullableString(dataReader["parent_department_name"])
                        });
                    }
                    return departments;
                }, _sqlHelper.CreateParam("@parentDepartmentId", parentDepartmentId));
            });
        }

        /// <summary>
        /// Creates a new department in the database
        /// </summary>
        public async Task<Guid> CreateDepartmentAsync(CreateDepartmentDto department)
        {
            Guid newId = Guid.NewGuid();
            string query = @"
                INSERT INTO tck.departments (id, name, department_type_id, parent_department_id)
                VALUES (@id, @name, @departmentTypeId, @parentDepartmentId)";

            return await Task.Run(() =>
            {
                var parentParam = department.ParentDepartmentId.HasValue
                    ? _sqlHelper.CreateParam("@parentDepartmentId", department.ParentDepartmentId.Value)
                    : _sqlHelper.CreateParam("@parentDepartmentId", DBNull.Value, NpgsqlTypes.NpgsqlDbType.Uuid);

                _sqlHelper.ExecuteData(query,
                    _sqlHelper.CreateParam("@id", newId),
                    _sqlHelper.CreateParam("@name", department.Name),
                    _sqlHelper.CreateParam("@departmentTypeId", department.DepartmentTypeId),
                    parentParam);
                return newId;
            });
        }

        /// <summary>
        /// Updates an existing department
        /// </summary>
        public async Task<bool> UpdateDepartmentAsync(UpdateDepartmentDto department)
        {
            string query = @"
                UPDATE tck.departments 
                SET name = @name,
                    department_type_id = @departmentTypeId,
                    parent_department_id = @parentDepartmentId
                WHERE id = @id";

            return await Task.Run(() =>
            {
                var parentParam = department.ParentDepartmentId.HasValue
                    ? _sqlHelper.CreateParam("@parentDepartmentId", department.ParentDepartmentId.Value)
                    : _sqlHelper.CreateParam("@parentDepartmentId", DBNull.Value, NpgsqlTypes.NpgsqlDbType.Uuid);

                int rowsAffected = _sqlHelper.ExecuteData(query,
                    _sqlHelper.CreateParam("@id", department.Id),
                    _sqlHelper.CreateParam("@name", department.Name),
                    _sqlHelper.CreateParam("@departmentTypeId", department.DepartmentTypeId),
                    parentParam);
                return rowsAffected > 0;
            });
        }

        /// <summary>
        /// Deletes a department from the database
        /// </summary>
        public async Task<bool> DeleteDepartmentAsync(Guid id)
        {
            string query = "DELETE FROM tck.departments WHERE id = @id";

            return await Task.Run(() =>
            {
                int rowsAffected = _sqlHelper.ExecuteData(query,
                    _sqlHelper.CreateParam("@id", id));
                return rowsAffected > 0;
            });
        }
    }
}
