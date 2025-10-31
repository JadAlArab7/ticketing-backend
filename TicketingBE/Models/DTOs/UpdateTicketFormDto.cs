namespace TicketingBE.Models.DTOs
{
    public class UpdateTicketFormDto
    {
        public string TicketTypeId { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? AlertBuffer { get; set; }
        public DateTime? Deadline { get; set; }
        public string TicketStatus { get; set; } = string.Empty;
        public string? AssigneeDepartmentId { get; set; }
    }
}
