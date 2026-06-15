using Logistics.Application.Common;
using Logistics.Application.DTOs.Deliveries;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Deliveries.Commands.AssignShipper;

public class AssignShipperCommand : IRequest<ApiResponse<DeliveryDto>>
{
    public int OrderId { get; }
    public AssignShipperDto AssignShipper { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public AssignShipperCommand(
        int orderId,
        AssignShipperDto assignShipper,
        int currentUserId,
        UserRole currentUserRole)
    {
        OrderId = orderId;
        AssignShipper = assignShipper;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
