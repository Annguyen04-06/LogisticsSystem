using Logistics.Application.Common;
using Logistics.Application.DTOs.Tickets;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Tickets.Commands.CloseTicket;

public class CloseTicketCommand : IRequest<ApiResponse<SupportTicketDto>>
{
    public int TicketId { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public CloseTicketCommand(int ticketId, int currentUserId, UserRole currentUserRole)
    {
        TicketId = ticketId;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
