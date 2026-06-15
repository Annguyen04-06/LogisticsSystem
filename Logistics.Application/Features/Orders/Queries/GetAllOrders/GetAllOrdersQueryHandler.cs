using Logistics.Application.Common;
using Logistics.Application.DTOs.Orders;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllOrdersQuery, ApiResponse<List<OrderDto>>>
{
    public async Task<ApiResponse<List<OrderDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<List<OrderDto>>.Fail("Only admin can view all orders.");
        }

        var orders = await OrderDtoBuilder.BuildListAsync(context, context.Orders, cancellationToken);
        return ApiResponse<List<OrderDto>>.Ok(orders);
    }
}
