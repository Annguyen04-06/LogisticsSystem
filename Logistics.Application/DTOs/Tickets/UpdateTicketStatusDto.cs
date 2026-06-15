using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.Tickets;

public class UpdateTicketStatusDto
{
    public TicketStatus Status { get; set; }
}
