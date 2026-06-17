using Logistics.Domain.Common;
using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class SupportTicket : BaseEntity
{
    public int CustomerId { get; set; }
    public int? SellerId { get; set; }
    public int? OrderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
}
