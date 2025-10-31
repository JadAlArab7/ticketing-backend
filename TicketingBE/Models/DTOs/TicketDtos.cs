namespace TicketingBE.Models.DTOs
{
    /// <summary>
    /// DTO for creating a new ticket
    /// </summary>
    public class CreateTicketDto
    {
        public string TicketTypeId { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? AlertBuffer { get; set; }
        public DateTime? Deadline { get; set; }
        public string? AssigneeDepartmentId { get; set; }
        public List<CreateTicketFileDto> Files { get; set; } = new();
    }

    /// <summary>
    /// DTO for updating an existing ticket
    /// </summary>
    public class UpdateTicketDto
    {
        public string Id { get; set; } = string.Empty;
        public string TicketTypeId { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? AlertBuffer { get; set; }
        public DateTime? Deadline { get; set; }
        public string TicketStatus { get; set; } = string.Empty;
        public List<CreateTicketAssigneeDto> Assignees { get; set; } = new();
        public List<CreateTicketFileDto> Files { get; set; } = new();
    }

    /// <summary>
    /// DTO for ticket detail response (including related data)
    /// </summary>
    public class TicketDetailDto
    {
        public string Id { get; set; } = string.Empty;
        public string TicketTypeId { get; set; } = string.Empty;
        public string TicketTypeName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? AlertBuffer { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TicketStatus { get; set; } = string.Empty;
        public string TicketStatusName { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByDepartmentName { get; set; } = string.Empty;
        public List<TicketAssigneeDto> Assignees { get; set; } = new();
        public List<TicketFileDto> Files { get; set; } = new();
    }

    /// <summary>
    /// DTO for ticket list item response
    /// </summary>
    public class TicketListItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string TicketTypeName { get; set; } = string.Empty;
        public string TicketStatusName { get; set; } = string.Empty;
        public string CreatedByDepartmentName { get; set; } = string.Empty;
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> AssignedToDepartments { get; set; } = new();
    }

    /// <summary>
    /// DTO for creating ticket assignee
    /// </summary>
    public class CreateTicketAssigneeDto
    {
        public string DepartmentId { get; set; } = string.Empty;
        public string TicketAssigneeType { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for ticket assignee response
    /// </summary>
    public class TicketAssigneeDto
    {
        public string TicketId { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string TicketAssigneeType { get; set; } = string.Empty;
        public string TicketAssigneeTypeName { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for creating ticket file
    /// </summary>
    public class CreateTicketFileDto
    {
        public string FileName { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public byte[] FileData { get; set; } = Array.Empty<byte>();
    }

    /// <summary>
    /// DTO for ticket file response
    /// </summary>
    public class TicketFileDto
    {
        public string Id { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public string TicketId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
