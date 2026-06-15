using Logistics.Application.Common;
using Logistics.Application.DTOs.Tickets;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Tickets.Commands.CloseTicket;

public class CloseTicketCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CloseTicketCommand, ApiResponse<SupportTicketDto>>
{
    public async Task<ApiResponse<SupportTicketDto>> Handle(CloseTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await context.SupportTickets
            .FirstOrDefaultAsync(ticket => ticket.Id == request.TicketId, cancellationToken);

        if (ticket is null)
        {
            return ApiResponse<SupportTicketDto>.Fail("Ticket does not exist.");
        }

        if (!HasTicketAccess(ticket, request.CurrentUserId, request.CurrentUserRole))
        {
            return ApiResponse<SupportTicketDto>.Fail("You do not have permission to close this ticket.");
        }

        ticket.Status = TicketStatus.Closed;
        ticket.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        var ticketDto = await TicketDtoBuilder.BuildOneAsync(
            context,
            context.SupportTickets.Where(item => item.Id == ticket.Id),
            cancellationToken);

        return ApiResponse<SupportTicketDto>.Ok(ticketDto!, "Ticket closed successfully.");
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
