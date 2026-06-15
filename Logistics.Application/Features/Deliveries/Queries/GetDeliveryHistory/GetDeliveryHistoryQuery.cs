using Logistics.Application.Common;
using Logistics.Application.DTOs.Deliveries;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Deliveries.Queries.GetDeliveryHistory;

public class GetDeliveryHistoryQuery : IRequest<ApiResponse<List<DeliveryDto>>>
{
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public GetDeliveryHistoryQuery(int currentUserId, UserRole currentUserRole)
    {
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
