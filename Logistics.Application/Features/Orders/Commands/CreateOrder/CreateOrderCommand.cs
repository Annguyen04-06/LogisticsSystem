using Logistics.Application.Common;
using Logistics.Application.DTOs.Orders;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<ApiResponse<List<OrderDto>>>
{
    public CreateOrderDto Order { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public CreateOrderCommand(CreateOrderDto order, int currentUserId, UserRole currentUserRole)
    {
        Order = order;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
