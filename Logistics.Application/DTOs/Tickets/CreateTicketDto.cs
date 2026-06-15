namespace Logistics.Application.DTOs.Tickets;

public class CreateTicketDto
{
    public int? SellerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
