using Logistics.Application.Common;
using Logistics.Application.DTOs.Tickets;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateTicketCommand, ApiResponse<SupportTicketDto>>
{
    public async Task<ApiResponse<SupportTicketDto>> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<SupportTicketDto>.Fail("Only customer can create support tickets.");
        }

        if (string.IsNullOrWhiteSpace(request.Ticket.Title))
        {
            return ApiResponse<SupportTicketDto>.Fail("Title is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Ticket.Content))
        {
            return ApiResponse<SupportTicketDto>.Fail("Content is required.");
        }

        if (request.Ticket.SellerId.HasValue)
        {
            var sellerExists = await context.Users
                .AnyAsync(
                    user => user.Id == request.Ticket.SellerId.Value && user.Role == UserRole.Seller,
                    cancellationToken);

            if (!sellerExists)
            {
                return ApiResponse<SupportTicketDto>.Fail("Seller does not exist.");
            }
        }

        var ticket = new SupportTicket
        {
            CustomerId = request.CurrentUserId,
            SellerId = request.Ticket.SellerId,
            Title = request.Ticket.Title.Trim(),
            Content = request.Ticket.Content.Trim(),
            Status = TicketStatus.Open
        };

        context.SupportTickets.Add(ticket);
        await context.SaveChangesAsync(cancellationToken);

        var ticketDto = await TicketDtoBuilder.BuildOneAsync(
            context,
            context.SupportTickets.Where(item => item.Id == ticket.Id),
            cancellationToken);

        return ApiResponse<SupportTicketDto>.Ok(ticketDto!, "Support ticket created successfully.");
    }
}
