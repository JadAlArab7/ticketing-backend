namespace TicketingBE.Models.DTOs
{
    /// <summary>
    /// DTO for creating a ticket via form data (supports file uploads)
    /// </summary>
    public class CreateTicketFormDto
    {
        public string TicketTypeId { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? AlertBuffer { get; set; }
        public DateTime? Deadline { get; set; }
        public string? AssigneeDepartmentId { get; set; }
        public List<IFormFile>? Files { get; set; }
    }

    /// <summary>
    /// Helper DTO for assignee data in form
    /// </summary>
    public class AssigneeFormData
    {
        public string DepartmentId { get; set; } = string.Empty;
        public string TicketAssigneeType { get; set; } = string.Empty;
    }
}
