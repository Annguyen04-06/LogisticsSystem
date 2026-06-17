using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.Tickets;

public class SupportTicketDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int? SellerId { get; set; }
    public string? SellerName { get; set; }
    public int? OrderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TicketReplyDto> Replies { get; set; } = [];
}
