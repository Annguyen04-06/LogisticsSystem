using Logistics.Application.Common;
using Logistics.Application.DTOs.Orders;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Orders.Queries.GetSellerOrders;

public class GetSellerOrdersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSellerOrdersQuery, ApiResponse<List<OrderDto>>>
{
    public async Task<ApiResponse<List<OrderDto>>> Handle(GetSellerOrdersQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Seller and not UserRole.Admin)
        {
            return ApiResponse<List<OrderDto>>.Fail("Only seller or admin can view seller orders.");
        }

        var ordersQuery = request.CurrentUserRole == UserRole.Admin
            ? context.Orders
            : context.Orders.Where(order => order.SellerId == request.CurrentUserId);

        var orders = await OrderDtoBuilder.BuildListAsync(context, ordersQuery, cancellationToken);
        return ApiResponse<List<OrderDto>>.Ok(orders);
    }
}
