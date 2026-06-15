using Logistics.Application.Common;
using Logistics.Application.DTOs.Deliveries;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Deliveries.Commands.UpdateDeliveryStatus;

public class UpdateDeliveryStatusCommand : IRequest<ApiResponse<DeliveryDto>>
{
    public int DeliveryId { get; }
    public UpdateDeliveryStatusDto DeliveryStatus { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public UpdateDeliveryStatusCommand(
        int deliveryId,
        UpdateDeliveryStatusDto deliveryStatus,
        int currentUserId,
        UserRole currentUserRole)
    {
        DeliveryId = deliveryId;
        DeliveryStatus = deliveryStatus;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
