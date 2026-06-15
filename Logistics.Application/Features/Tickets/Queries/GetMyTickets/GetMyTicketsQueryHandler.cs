using Logistics.Application.Common;
using Logistics.Application.DTOs.Tickets;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Tickets.Queries.GetMyTickets;

public class GetMyTicketsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMyTicketsQuery, ApiResponse<List<SupportTicketDto>>>
{
    public async Task<ApiResponse<List<SupportTicketDto>>> Handle(GetMyTicketsQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<List<SupportTicketDto>>.Fail("Only customer can view my tickets.");
        }

        var tickets = await TicketDtoBuilder.BuildListAsync(
            context,
            context.SupportTickets.Where(ticket => ticket.CustomerId == request.CurrentUserId),
            cancellationToken);

        return ApiResponse<List<SupportTicketDto>>.Ok(tickets);
    }
}
