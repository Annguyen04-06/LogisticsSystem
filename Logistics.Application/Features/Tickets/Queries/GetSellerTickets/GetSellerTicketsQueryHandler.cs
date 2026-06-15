using Logistics.Application.Common;
using Logistics.Application.DTOs.Tickets;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Tickets.Queries.GetSellerTickets;

public class GetSellerTicketsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSellerTicketsQuery, ApiResponse<List<SupportTicketDto>>>
{
    public async Task<ApiResponse<List<SupportTicketDto>>> Handle(GetSellerTicketsQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Seller and not UserRole.Admin)
        {
            return ApiResponse<List<SupportTicketDto>>.Fail("Only seller or admin can view seller tickets.");
        }

        var ticketsQuery = request.CurrentUserRole == UserRole.Admin
            ? context.SupportTickets
            : context.SupportTickets.Where(ticket => ticket.SellerId == request.CurrentUserId);

        var tickets = await TicketDtoBuilder.BuildListAsync(context, ticketsQuery, cancellationToken);
        return ApiResponse<List<SupportTicketDto>>.Ok(tickets);
    }
}
