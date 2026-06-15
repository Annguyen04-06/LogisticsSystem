using Logistics.Application.Common;
using Logistics.Application.DTOs.Carts;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Carts.Commands.AddToCart;

public class AddToCartCommand : IRequest<ApiResponse<CartDto>>
{
    public AddToCartDto CartItem { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public AddToCartCommand(AddToCartDto cartItem, int currentUserId, UserRole currentUserRole)
    {
        CartItem = cartItem;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
