using Logistics.Application.Common;
using Logistics.Application.DTOs.Orders;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQuery : IRequest<ApiResponse<List<OrderDto>>>
{
    public UserRole CurrentUserRole { get; }

    public GetAllOrdersQuery(UserRole currentUserRole)
    {
        CurrentUserRole = currentUserRole;
    }
}
