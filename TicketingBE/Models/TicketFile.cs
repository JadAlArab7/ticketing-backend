namespace TicketingBE.Models
{
    /// <summary>
    /// Represents a Ticket File entity in the system (DAL Model)
    /// </summary>
    public class TicketFile
    {
        public string Id { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public byte[] FileData { get; set; } = Array.Empty<byte>();
        public string TicketId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
