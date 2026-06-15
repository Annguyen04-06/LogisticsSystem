using Logistics.Application.Common;
using Logistics.Application.DTOs.Tickets;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Tickets.Commands.UpdateTicketStatus;

public class UpdateTicketStatusCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateTicketStatusCommand, ApiResponse<SupportTicketDto>>
{
    public async Task<ApiResponse<SupportTicketDto>> Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Seller and not UserRole.Admin)
        {
            return ApiResponse<SupportTicketDto>.Fail("Only seller or admin can update ticket status.");
        }

        if (!Enum.IsDefined(request.TicketStatus.Status))
        {
            return ApiResponse<SupportTicketDto>.Fail("Ticket status is invalid.");
        }

        var ticket = await context.SupportTickets
            .FirstOrDefaultAsync(ticket => ticket.Id == request.TicketId, cancellationToken);

        if (ticket is null)
        {
            return ApiResponse<SupportTicketDto>.Fail("Ticket does not exist.");
        }

        if (request.CurrentUserRole == UserRole.Seller && ticket.SellerId != request.CurrentUserId)
        {
            return ApiResponse<SupportTicketDto>.Fail("Seller can only update related tickets.");
        }

        ticket.Status = request.TicketStatus.Status;
        ticket.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        var ticketDto = await TicketDtoBuilder.BuildOneAsync(
            context,
            context.SupportTickets.Where(item => item.Id == ticket.Id),
            cancellationToken);

        return ApiResponse<SupportTicketDto>.Ok(ticketDto!, "Ticket status updated successfully.");
    }
}
