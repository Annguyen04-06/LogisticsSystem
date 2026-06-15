using Logistics.Application.Common;
using Logistics.Application.DTOs.Tickets;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Tickets.Queries.GetTicketById;

public class GetTicketByIdQuery : IRequest<ApiResponse<SupportTicketDto>>
{
    public int TicketId { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public GetTicketByIdQuery(int ticketId, int currentUserId, UserRole currentUserRole)
    {
        TicketId = ticketId;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
