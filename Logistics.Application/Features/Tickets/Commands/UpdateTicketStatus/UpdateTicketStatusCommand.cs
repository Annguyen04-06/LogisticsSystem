using Logistics.Application.Common;
using Logistics.Application.DTOs.Tickets;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Tickets.Commands.UpdateTicketStatus;

public class UpdateTicketStatusCommand : IRequest<ApiResponse<SupportTicketDto>>
{
    public int TicketId { get; }
    public UpdateTicketStatusDto TicketStatus { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public UpdateTicketStatusCommand(
        int ticketId,
        UpdateTicketStatusDto ticketStatus,
        int currentUserId,
        UserRole currentUserRole)
    {
        TicketId = ticketId;
        TicketStatus = ticketStatus;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
