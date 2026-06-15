using Logistics.Application.Common;
using Logistics.Application.DTOs.Tickets;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Tickets.Commands.ReplyTicket;

public class ReplyTicketCommand : IRequest<ApiResponse<SupportTicketDto>>
{
    public int TicketId { get; }
    public ReplyTicketDto Reply { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public ReplyTicketCommand(int ticketId, ReplyTicketDto reply, int currentUserId, UserRole currentUserRole)
    {
        TicketId = ticketId;
        Reply = reply;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
