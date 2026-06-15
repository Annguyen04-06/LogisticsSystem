using Logistics.Application.Common;
using Logistics.Application.DTOs.Orders;
using Logistics.Application.Features.Orders;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Deliveries.Commands.ConfirmOrder;

public class ConfirmOrderCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ConfirmOrderCommand, ApiResponse<OrderDto>>
{
    public async Task<ApiResponse<OrderDto>> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Seller and not UserRole.Admin)
        {
            return ApiResponse<OrderDto>.Fail("Only seller or admin can confirm orders.");
        }

        var order = await context.Orders
            .FirstOrDefaultAsync(order => order.Id == request.OrderId, cancellationToken);

        if (order is null)
        {
            return ApiResponse<OrderDto>.Fail("Order does not exist.");
        }

        if (request.CurrentUserRole == UserRole.Seller && order.SellerId != request.CurrentUserId)
        {
            return ApiResponse<OrderDto>.Fail("Seller can only confirm their own order.");
        }

        if (order.Status != OrderStatus.Pending)
        {
            return ApiResponse<OrderDto>.Fail("Only pending orders can be confirmed.");
        }

        order.Status = OrderStatus.Confirmed;
        order.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        var orderDto = await OrderDtoBuilder.BuildOneAsync(
            context,
            context.Orders.Where(item => item.Id == order.Id),
            cancellationToken);

        return ApiResponse<OrderDto>.Ok(orderDto!, "Order confirmed successfully.");
    }
}
