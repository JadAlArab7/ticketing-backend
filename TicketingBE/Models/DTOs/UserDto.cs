namespace TicketingBE.Models.DTOs
{
    /// <summary>
    /// DTO for user information returned by APIs
    /// </summary>
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
    }
}
