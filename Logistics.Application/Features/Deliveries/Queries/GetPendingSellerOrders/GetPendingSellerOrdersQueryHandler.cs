using Logistics.Application.Common;
using Logistics.Application.DTOs.Orders;
using Logistics.Application.Features.Orders;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Deliveries.Queries.GetPendingSellerOrders;

public class GetPendingSellerOrdersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetPendingSellerOrdersQuery, ApiResponse<List<OrderDto>>>
{
    public async Task<ApiResponse<List<OrderDto>>> Handle(GetPendingSellerOrdersQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Seller and not UserRole.Admin)
        {
            return ApiResponse<List<OrderDto>>.Fail("Only seller or admin can view seller orders.");
        }

        var ordersQuery = context.Orders
            .Where(order => order.Status == OrderStatus.Pending || order.Status == OrderStatus.Confirmed);

        if (request.CurrentUserRole == UserRole.Seller)
        {
            ordersQuery = ordersQuery.Where(order => order.SellerId == request.CurrentUserId);
        }

        var orders = await OrderDtoBuilder.BuildListAsync(context, ordersQuery, cancellationToken);
        return ApiResponse<List<OrderDto>>.Ok(orders);
    }
}
