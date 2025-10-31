namespace TicketingBE.Models.DTOs
{
    /// <summary>
    /// DTO for login response including JWT token and user details
    /// </summary>
    public class LoginResponseDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentType { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
