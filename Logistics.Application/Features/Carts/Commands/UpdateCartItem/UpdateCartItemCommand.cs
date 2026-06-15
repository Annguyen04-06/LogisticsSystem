using Logistics.Application.Common;
using Logistics.Application.DTOs.Carts;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Carts.Commands.UpdateCartItem;

public class UpdateCartItemCommand : IRequest<ApiResponse<CartDto>>
{
    public int CartItemId { get; }
    public UpdateCartItemDto CartItem { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public UpdateCartItemCommand(
        int cartItemId,
        UpdateCartItemDto cartItem,
        int currentUserId,
        UserRole currentUserRole)
    {
        CartItemId = cartItemId;
        CartItem = cartItem;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
