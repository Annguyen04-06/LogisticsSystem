using Logistics.Application.Common;
using Logistics.Application.DTOs.Orders;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommand : IRequest<ApiResponse<OrderDto>>
{
    public int OrderId { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public CancelOrderCommand(int orderId, int currentUserId, UserRole currentUserRole)
    {
        OrderId = orderId;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
