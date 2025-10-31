using TicketingBE.BLL.Interfaces;
using TicketingBE.DAL.Interfaces;
using TicketingBE.Models;
using TicketingBE.Models.DTOs;

namespace TicketingBE.BLL.Services
{
    /// <summary>
    /// Implementation of IDepartmentService containing business logic for Department operations
    /// This layer handles validation, business rules, and coordinates between Controller and DAL
    /// </summary>
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(IDepartmentRepository departmentRepository, ILogger<DepartmentService> logger)
        {
            _departmentRepository = departmentRepository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all departments
        /// </summary>
        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            try
            {
                return await _departmentRepository.GetAllDepartmentsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a specific department by its ID
        /// </summary>
        public async Task<Department?> GetDepartmentByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid department ID: {DepartmentId}", id);
                    return null;
                }

                return await _departmentRepository.GetDepartmentByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving department with ID: {DepartmentId}", id);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all departments by department type
        /// </summary>
        public async Task<IEnumerable<Department>> GetDepartmentsByTypeAsync(Guid departmentTypeId)
        {
            try
            {
                if (departmentTypeId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid department type ID: {DepartmentTypeId}", departmentTypeId);
                    return Enumerable.Empty<Department>();
                }

                return await _departmentRepository.GetDepartmentsByTypeAsync(departmentTypeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments by type: {DepartmentTypeId}", departmentTypeId);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all child departments of a parent department
        /// </summary>
        public async Task<IEnumerable<Department>> GetChildDepartmentsAsync(Guid parentDepartmentId)
        {
            try
            {
                if (parentDepartmentId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid parent department ID: {ParentDepartmentId}", parentDepartmentId);
                    return Enumerable.Empty<Department>();
                }

                return await _departmentRepository.GetChildDepartmentsAsync(parentDepartmentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving child departments for parent: {ParentDepartmentId}", parentDepartmentId);
                throw;
            }
        }

        /// <summary>
        /// Creates a new department with validation
        /// Business Rules: Validate required fields, department type exists, parent department exists (if provided)
        /// </summary>
        public async Task<Guid> CreateDepartmentAsync(CreateDepartmentDto department)
        {
            try
            {
                // Business rule: Validate required fields
                if (string.IsNullOrWhiteSpace(department.Name))
                    throw new ArgumentException("Department name is required");

                if (department.DepartmentTypeId == Guid.Empty)
                    throw new ArgumentException("Department type is required");

                // Business rule: Validate parent department exists if provided
                if (department.ParentDepartmentId.HasValue && department.ParentDepartmentId.Value != Guid.Empty)
                {
                    var parentDepartment = await _departmentRepository.GetDepartmentByIdAsync(department.ParentDepartmentId.Value);
                    if (parentDepartment == null)
                    {
                        throw new ArgumentException($"Parent department with ID {department.ParentDepartmentId.Value} does not exist");
                    }
                }

                return await _departmentRepository.CreateDepartmentAsync(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing department with validation
        /// Business Rules: Validate required fields, department exists, department type exists, parent department exists (if provided)
        /// </summary>
        public async Task<bool> UpdateDepartmentAsync(UpdateDepartmentDto department)
        {
            try
            {
                // Business rule: Validate required fields
                if (department.Id == Guid.Empty)
                    throw new ArgumentException("Department ID is required");

                if (string.IsNullOrWhiteSpace(department.Name))
                    throw new ArgumentException("Department name is required");

                if (department.DepartmentTypeId == Guid.Empty)
                    throw new ArgumentException("Department type is required");

                // Business rule: Validate department exists
                var existingDepartment = await _departmentRepository.GetDepartmentByIdAsync(department.Id);
                if (existingDepartment == null)
                {
                    throw new ArgumentException($"Department with ID {department.Id} does not exist");
                }

                // Business rule: Validate parent department exists if provided
                if (department.ParentDepartmentId.HasValue && department.ParentDepartmentId.Value != Guid.Empty)
                {
                    var parentDepartment = await _departmentRepository.GetDepartmentByIdAsync(department.ParentDepartmentId.Value);
                    if (parentDepartment == null)
                    {
                        throw new ArgumentException($"Parent department with ID {department.ParentDepartmentId.Value} does not exist");
                    }

                    // Business rule: Cannot set itself as parent
                    if (department.ParentDepartmentId.Value == department.Id)
                    {
                        throw new ArgumentException("A department cannot be its own parent");
                    }
                }

                return await _departmentRepository.UpdateDepartmentAsync(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department with ID: {DepartmentId}", department.Id);
                throw;
            }
        }

        /// <summary>
        /// Deletes a department
        /// Business Rules: Department must exist, cannot delete if it has child departments or users
        /// </summary>
        public async Task<bool> DeleteDepartmentAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid department ID: {DepartmentId}", id);
                    return false;
                }

                // Business rule: Validate department exists
                var department = await _departmentRepository.GetDepartmentByIdAsync(id);
                if (department == null)
                {
                    throw new ArgumentException($"Department with ID {id} does not exist");
                }

                // Business rule: Check if department has child departments
                var childDepartments = await _departmentRepository.GetChildDepartmentsAsync(id);
                if (childDepartments.Any())
                {
                    throw new InvalidOperationException($"Cannot delete department with ID {id} because it has child departments");
                }

                return await _departmentRepository.DeleteDepartmentAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department with ID: {DepartmentId}", id);
                throw;
            }
        }
    }
}
