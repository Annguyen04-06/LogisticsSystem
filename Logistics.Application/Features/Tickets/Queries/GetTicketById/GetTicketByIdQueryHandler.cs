using Logistics.Application.Common;
using Logistics.Application.DTOs.Tickets;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Tickets.Queries.GetTicketById;

public class GetTicketByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetTicketByIdQuery, ApiResponse<SupportTicketDto>>
{
    public async Task<ApiResponse<SupportTicketDto>> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var ticket = await context.SupportTickets
            .FirstOrDefaultAsync(ticket => ticket.Id == request.TicketId, cancellationToken);

        if (ticket is null)
        {
            return ApiResponse<SupportTicketDto>.Fail("Ticket does not exist.");
        }

        if (!HasTicketAccess(ticket, request.CurrentUserId, request.CurrentUserRole))
        {
            return ApiResponse<SupportTicketDto>.Fail("You do not have permission to view this ticket.");
        }

        var ticketDto = await TicketDtoBuilder.BuildOneAsync(
            context,
            context.SupportTickets.Where(item => item.Id == ticket.Id),
            cancellationToken);

        return ApiResponse<SupportTicketDto>.Ok(ticketDto!);
    }

    private static bool HasTicketAccess(SupportTicket ticket, int currentUserId, UserRole currentUserRole)
    {
        return currentUserRole switch
        {
            UserRole.Admin => true,
            UserRole.Customer => ticket.CustomerId == currentUserId,
            UserRole.Seller => ticket.SellerId == currentUserId,
            _ => false
        };
    }
}
