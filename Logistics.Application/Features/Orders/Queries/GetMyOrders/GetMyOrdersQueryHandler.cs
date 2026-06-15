using Logistics.Application.Common;
using Logistics.Application.DTOs.Orders;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Orders.Queries.GetMyOrders;

public class GetMyOrdersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMyOrdersQuery, ApiResponse<List<OrderDto>>>
{
    public async Task<ApiResponse<List<OrderDto>>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<List<OrderDto>>.Fail("Only customer can view my orders.");
        }

        var orders = await OrderDtoBuilder.BuildListAsync(
            context,
            context.Orders.Where(order => order.CustomerId == request.CurrentUserId),
            cancellationToken);

        return ApiResponse<List<OrderDto>>.Ok(orders);
    }
}
