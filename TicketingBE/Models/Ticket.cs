namespace TicketingBE.Models
{
    /// <summary>
    /// Represents a Ticket entity in the system (DAL Model)
    /// </summary>
    public class Ticket
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
    }
}
