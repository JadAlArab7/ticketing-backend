using TicketingBE.Models;
using TicketingBE.Models.DTOs;

namespace TicketingBE.BLL.Interfaces
{
    /// <summary>
    /// Interface defining business logic operations for Department entity
    /// </summary>
    public interface IDepartmentService
    {
        /// <summary>
        /// Retrieves all departments
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
        /// Creates a new department with validation
        /// </summary>
        Task<Guid> CreateDepartmentAsync(CreateDepartmentDto department);

        /// <summary>
        /// Updates an existing department with validation
        /// </summary>
        Task<bool> UpdateDepartmentAsync(UpdateDepartmentDto department);

        /// <summary>
        /// Deletes a department
        /// </summary>
        Task<bool> DeleteDepartmentAsync(Guid id);
    }
}
