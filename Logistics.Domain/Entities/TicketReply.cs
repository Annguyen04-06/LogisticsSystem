using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class TicketReply : BaseEntity
{
    public int SupportTicketId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = string.Empty;
}
