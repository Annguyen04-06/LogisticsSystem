using Logistics.Application.Common;
using Logistics.Application.DTOs.Tickets;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Tickets.Commands.ReplyTicket;

public class ReplyTicketCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ReplyTicketCommand, ApiResponse<SupportTicketDto>>
{
    public async Task<ApiResponse<SupportTicketDto>> Handle(ReplyTicketCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Reply.Message))
        {
            return ApiResponse<SupportTicketDto>.Fail("Message is required.");
        }

        var ticket = await context.SupportTickets
            .FirstOrDefaultAsync(ticket => ticket.Id == request.TicketId, cancellationToken);

        if (ticket is null)
        {
            return ApiResponse<SupportTicketDto>.Fail("Ticket does not exist.");
        }

        if (ticket.Status == TicketStatus.Closed)
        {
            return ApiResponse<SupportTicketDto>.Fail("Closed tickets cannot be replied to.");
        }

        if (!HasTicketAccess(ticket, request.CurrentUserId, request.CurrentUserRole))
        {
            return ApiResponse<SupportTicketDto>.Fail("You do not have permission to reply to this ticket.");
        }

        context.TicketReplies.Add(new TicketReply
        {
            SupportTicketId = ticket.Id,
            UserId = request.CurrentUserId,
            Content = request.Reply.Message.Trim()
        });

        if (request.CurrentUserRole is UserRole.Seller or UserRole.Admin && ticket.Status == TicketStatus.Open)
        {
            ticket.Status = TicketStatus.InProgress;
            ticket.UpdatedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(cancellationToken);

        var ticketDto = await TicketDtoBuilder.BuildOneAsync(
            context,
            context.SupportTickets.Where(item => item.Id == ticket.Id),
            cancellationToken);

        return ApiResponse<SupportTicketDto>.Ok(ticketDto!, "Reply added successfully.");
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
