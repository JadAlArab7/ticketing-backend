namespace TicketingBE.Models
{
    /// <summary>
    /// Represents a Department entity in the system (DAL Model)
    /// </summary>
    public class Department
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid DepartmentTypeId { get; set; }
        public string DepartmentTypeName { get; set; } = string.Empty;
        public Guid? ParentDepartmentId { get; set; }
        public string? ParentDepartmentName { get; set; }
    }
}
