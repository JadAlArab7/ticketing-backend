namespace TicketingBE.Models.DTOs
{
    /// <summary>
    /// DTO for creating a new department
    /// </summary>
    public class CreateDepartmentDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid DepartmentTypeId { get; set; }
        public Guid? ParentDepartmentId { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing department
    /// </summary>
    public class UpdateDepartmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid DepartmentTypeId { get; set; }
        public Guid? ParentDepartmentId { get; set; }
    }
}
