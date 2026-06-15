using Logistics.Application.Common;
using Logistics.Application.DTOs.Orders;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Orders.Queries.GetMyOrders;

public class GetMyOrdersQuery : IRequest<ApiResponse<List<OrderDto>>>
{
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public GetMyOrdersQuery(int currentUserId, UserRole currentUserRole)
    {
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
