using Logistics.Application.Common;
using Logistics.Application.DTOs.Tickets;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Tickets.Queries.GetAllTickets;

public class GetAllTicketsQuery : IRequest<ApiResponse<List<SupportTicketDto>>>
{
    public UserRole CurrentUserRole { get; }

    public GetAllTicketsQuery(UserRole currentUserRole)
    {
        CurrentUserRole = currentUserRole;
    }
}
