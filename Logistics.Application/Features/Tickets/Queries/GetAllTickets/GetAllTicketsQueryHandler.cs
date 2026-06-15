using Logistics.Application.Common;
using Logistics.Application.DTOs.Tickets;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Tickets.Queries.GetAllTickets;

public class GetAllTicketsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllTicketsQuery, ApiResponse<List<SupportTicketDto>>>
{
    public async Task<ApiResponse<List<SupportTicketDto>>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<List<SupportTicketDto>>.Fail("Only admin can view all tickets.");
        }

        var tickets = await TicketDtoBuilder.BuildListAsync(context, context.SupportTickets, cancellationToken);
        return ApiResponse<List<SupportTicketDto>>.Ok(tickets);
    }
}
