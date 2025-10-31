using TicketingBE.Models;

namespace TicketingBE.DAL.Interfaces
{
    /// <summary>
    /// Repository interface for Ticket Assignee entity operations
    /// </summary>
    public interface ITicketAssigneeRepository
    {
        Task<IEnumerable<TicketAssignee>> GetAssigneesByTicketIdAsync(string ticketId);
        Task<bool> AddAssigneeAsync(string ticketId, string departmentId, string ticketAssigneeType);
        Task<bool> RemoveAssigneeAsync(string ticketId, string departmentId);
        Task<bool> RemoveAllAssigneesByTicketIdAsync(string ticketId);
    }
}
