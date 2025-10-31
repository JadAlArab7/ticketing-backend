namespace TicketingBE.Models
{
    /// <summary>
    /// Represents a User entity in the system (DAL Model)
    /// </summary>
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentType { get; set; } = string.Empty;
    }
}
