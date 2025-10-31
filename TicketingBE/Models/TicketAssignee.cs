namespace TicketingBE.Models
{
    /// <summary>
    /// Represents a Ticket Assignee entity in the system (DAL Model)
    /// </summary>
    public class TicketAssignee
    {
        public string TicketId { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string TicketAssigneeType { get; set; } = string.Empty;
        public string TicketAssigneeTypeName { get; set; } = string.Empty;
    }
}
