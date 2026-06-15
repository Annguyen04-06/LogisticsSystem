using Logistics.Application.Common;
using Logistics.Application.DTOs.Carts;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Carts.Commands.RemoveCartItem;

public class RemoveCartItemCommand : IRequest<ApiResponse<CartDto>>
{
    public int CartItemId { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public RemoveCartItemCommand(int cartItemId, int currentUserId, UserRole currentUserRole)
    {
        CartItemId = cartItemId;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
