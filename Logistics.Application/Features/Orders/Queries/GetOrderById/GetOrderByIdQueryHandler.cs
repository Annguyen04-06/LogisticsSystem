using Logistics.Application.Common;
using Logistics.Application.DTOs.Orders;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetOrderByIdQuery, ApiResponse<OrderDto>>
{
    public async Task<ApiResponse<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .FirstOrDefaultAsync(order => order.Id == request.OrderId, cancellationToken);

        if (order is null)
        {
            return ApiResponse<OrderDto>.Fail("Order does not exist.");
        }

        var hasPermission = request.CurrentUserRole switch
        {
            UserRole.Admin => true,
            UserRole.Customer => order.CustomerId == request.CurrentUserId,
            UserRole.Seller => order.SellerId == request.CurrentUserId,
            _ => false
        };

        if (!hasPermission)
        {
            return ApiResponse<OrderDto>.Fail("You do not have permission to view this order.");
        }

        var orderDto = await OrderDtoBuilder.BuildOneAsync(
            context,
            context.Orders.Where(item => item.Id == order.Id),
            cancellationToken);

        return ApiResponse<OrderDto>.Ok(orderDto!);
    }
}
