using Logistics.Application.Common;
using Logistics.Application.DTOs.Orders;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Deliveries.Queries.GetPendingSellerOrders;

public class GetPendingSellerOrdersQuery : IRequest<ApiResponse<List<OrderDto>>>
{
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public GetPendingSellerOrdersQuery(int currentUserId, UserRole currentUserRole)
    {
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
