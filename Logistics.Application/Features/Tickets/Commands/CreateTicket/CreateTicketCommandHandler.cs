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

        int? sellerId = request.Ticket.SellerId;

        if (request.Ticket.OrderId.HasValue)
        {
            var order = await context.Orders
                .FirstOrDefaultAsync(
                    order => order.Id == request.Ticket.OrderId.Value,
                    cancellationToken);

            if (order is null || order.CustomerId != request.CurrentUserId)
            {
                return ApiResponse<SupportTicketDto>.Fail("Bạn không có quyền gửi hỗ trợ cho đơn hàng này.");
            }

            sellerId = order.SellerId;
        }

        if (sellerId.HasValue)
        {
            var seller = await context.Users
                .FirstOrDefaultAsync(user => user.Id == sellerId.Value, cancellationToken);

            if (seller is null || seller.Role != UserRole.Seller)
            {
                return ApiResponse<SupportTicketDto>.Fail("Tài khoản được chọn không phải là người bán.");
            }
        }

        var ticket = new SupportTicket
        {
            CustomerId = request.CurrentUserId,
            SellerId = sellerId,
            OrderId = request.Ticket.OrderId,
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
