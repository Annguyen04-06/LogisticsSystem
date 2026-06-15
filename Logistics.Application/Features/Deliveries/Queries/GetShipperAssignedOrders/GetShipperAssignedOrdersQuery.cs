using Logistics.Application.Common;
using Logistics.Application.DTOs.Deliveries;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Deliveries.Queries.GetShipperAssignedOrders;

public class GetShipperAssignedOrdersQuery : IRequest<ApiResponse<List<DeliveryDto>>>
{
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public GetShipperAssignedOrdersQuery(int currentUserId, UserRole currentUserRole)
    {
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
