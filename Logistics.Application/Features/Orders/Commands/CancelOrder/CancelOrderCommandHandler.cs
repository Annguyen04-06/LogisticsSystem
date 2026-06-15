using Logistics.Application.Common;
using Logistics.Application.DTOs.Orders;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CancelOrderCommand, ApiResponse<OrderDto>>
{
    public async Task<ApiResponse<OrderDto>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Customer and not UserRole.Admin)
        {
            return ApiResponse<OrderDto>.Fail("Only customer or admin can cancel orders.");
        }

        var order = await context.Orders
            .FirstOrDefaultAsync(order => order.Id == request.OrderId, cancellationToken);

        if (order is null)
        {
            return ApiResponse<OrderDto>.Fail("Order does not exist.");
        }

        if (request.CurrentUserRole == UserRole.Customer)
        {
            if (order.CustomerId != request.CurrentUserId)
            {
                return ApiResponse<OrderDto>.Fail("You cannot cancel another customer's order.");
            }

            if (order.Status != OrderStatus.Pending)
            {
                return ApiResponse<OrderDto>.Fail("Customer can only cancel pending orders.");
            }
        }

        if (request.CurrentUserRole == UserRole.Admin &&
            order.Status is not OrderStatus.Pending and not OrderStatus.Confirmed)
        {
            return ApiResponse<OrderDto>.Fail("Admin can only cancel pending or confirmed orders.");
        }

        var details = await context.OrderDetails
            .Where(detail => detail.OrderId == order.Id)
            .ToListAsync(cancellationToken);

        var productIds = details.Select(detail => detail.ProductId).ToList();
        var products = await context.Products
            .Where(product => productIds.Contains(product.Id))
            .ToListAsync(cancellationToken);

        foreach (var detail in details)
        {
            var product = products.FirstOrDefault(product => product.Id == detail.ProductId);

            if (product is null)
            {
                continue;
            }

            product.Quantity += detail.Quantity;
            product.UpdatedAt = DateTime.UtcNow;

            context.StockTransactions.Add(new StockTransaction
            {
                ProductId = product.Id,
                Quantity = detail.Quantity,
                Type = "Import",
                Note = "Order cancelled"
            });
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        var orderDto = await OrderDtoBuilder.BuildOneAsync(
            context,
            context.Orders.Where(item => item.Id == order.Id),
            cancellationToken);

        return ApiResponse<OrderDto>.Ok(orderDto!, "Order cancelled successfully.");
    }
}
