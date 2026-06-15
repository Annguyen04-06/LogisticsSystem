using Logistics.Application.Common;
using Logistics.Application.DTOs.Tickets;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Tickets.Queries.GetMyTickets;

public class GetMyTicketsQuery : IRequest<ApiResponse<List<SupportTicketDto>>>
{
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public GetMyTicketsQuery(int currentUserId, UserRole currentUserRole)
    {
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
