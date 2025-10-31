using Microsoft.AspNetCore.Mvc;
using TicketingBE.BLL.Interfaces;
using TicketingBE.Models;
using TicketingBE.Models.DTOs;

namespace TicketingBE.Controllers
{
    /// <summary>
    /// API Controller for Department operations
    /// This layer handles HTTP requests/responses and communicates with the Business Logic Layer
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(IDepartmentService departmentService, ILogger<DepartmentController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }

        /// <summary>
        /// GET: api/Department
        /// Retrieves all departments
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetAllDepartments()
        {
            try
            {
                var departments = await _departmentService.GetAllDepartmentsAsync();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments");
                return StatusCode(500, "An error occurred while retrieving departments");
            }
        }

        /// <summary>
        /// GET: api/Department/{id}
        /// Retrieves a specific department by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartmentById(Guid id)
        {
            try
            {
                var department = await _departmentService.GetDepartmentByIdAsync(id);

                if (department == null)
                    return NotFound($"Department with ID {id} not found");

                return Ok(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving department with ID: {DepartmentId}", id);
                return StatusCode(500, "An error occurred while retrieving the department");
            }
        }

        /// <summary>
        /// GET: api/Department/type/{departmentTypeId}
        /// Retrieves all departments by department type
        /// </summary>
        [HttpGet("type/{departmentTypeId}")]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartmentsByType(Guid departmentTypeId)
        {
            try
            {
                var departments = await _departmentService.GetDepartmentsByTypeAsync(departmentTypeId);
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments by type: {DepartmentTypeId}", departmentTypeId);
                return StatusCode(500, "An error occurred while retrieving departments by type");
            }
        }

        /// <summary>
        /// GET: api/Department/children/{parentDepartmentId}
        /// Retrieves all child departments of a parent department
        /// </summary>
        [HttpGet("children/{parentDepartmentId}")]
        public async Task<ActionResult<IEnumerable<Department>>> GetChildDepartments(Guid parentDepartmentId)
        {
            try
            {
                var departments = await _departmentService.GetChildDepartmentsAsync(parentDepartmentId);
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving child departments for parent: {ParentDepartmentId}", parentDepartmentId);
                return StatusCode(500, "An error occurred while retrieving child departments");
            }
        }

        /// <summary>
        /// POST: api/Department
        /// Creates a new department
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Department>> CreateDepartment([FromBody] CreateDepartmentDto department)
        {
            try
            {
                var departmentId = await _departmentService.CreateDepartmentAsync(department);
                var createdDepartment = await _departmentService.GetDepartmentByIdAsync(departmentId);

                return CreatedAtAction(nameof(GetDepartmentById), new { id = departmentId }, createdDepartment);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating department");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                return StatusCode(500, "An error occurred while creating the department");
            }
        }

        /// <summary>
        /// PUT: api/Department/{id}
        /// Updates an existing department
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDepartment(Guid id, [FromBody] UpdateDepartmentDto department)
        {
            try
            {
                if (id != department.Id)
                    return BadRequest("ID in URL does not match ID in request body");

                var success = await _departmentService.UpdateDepartmentAsync(department);

                if (!success)
                    return NotFound($"Department with ID {id} not found");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error updating department with ID: {DepartmentId}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department with ID: {DepartmentId}", id);
                return StatusCode(500, "An error occurred while updating the department");
            }
        }

        /// <summary>
        /// DELETE: api/Department/{id}
        /// Deletes a department
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDepartment(Guid id)
        {
            try
            {
                var success = await _departmentService.DeleteDepartmentAsync(id);

                if (!success)
                    return NotFound($"Department with ID {id} not found");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error deleting department with ID: {DepartmentId}", id);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation deleting department with ID: {DepartmentId}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department with ID: {DepartmentId}", id);
                return StatusCode(500, "An error occurred while deleting the department");
            }
        }
    }
}
