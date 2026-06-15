using Logistics.Application.Common;
using Logistics.Application.DTOs.Orders;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Orders.Queries.GetSellerOrders;

public class GetSellerOrdersQuery : IRequest<ApiResponse<List<OrderDto>>>
{
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public GetSellerOrdersQuery(int currentUserId, UserRole currentUserRole)
    {
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
