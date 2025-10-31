using TicketingBE.Models;
using TicketingBE.Models.DTOs;

namespace TicketingBE.DAL.Interfaces
{
    /// <summary>
    /// Interface defining data access operations for Department entity
    /// </summary>
    public interface IDepartmentRepository
    {
        /// <summary>
        /// Retrieves all departments from the database
        /// </summary>
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();

        /// <summary>
        /// Retrieves a specific department by its ID
        /// </summary>
        Task<Department?> GetDepartmentByIdAsync(Guid id);

        /// <summary>
        /// Retrieves all departments by department type
        /// </summary>
        Task<IEnumerable<Department>> GetDepartmentsByTypeAsync(Guid departmentTypeId);

        /// <summary>
        /// Retrieves all child departments of a parent department
        /// </summary>
        Task<IEnumerable<Department>> GetChildDepartmentsAsync(Guid parentDepartmentId);

        /// <summary>
        /// Creates a new department in the database
        /// </summary>
        Task<Guid> CreateDepartmentAsync(CreateDepartmentDto department);

        /// <summary>
        /// Updates an existing department
        /// </summary>
        Task<bool> UpdateDepartmentAsync(UpdateDepartmentDto department);

        /// <summary>
        /// Deletes a department from the database
        /// </summary>
        Task<bool> DeleteDepartmentAsync(Guid id);
    }
}
