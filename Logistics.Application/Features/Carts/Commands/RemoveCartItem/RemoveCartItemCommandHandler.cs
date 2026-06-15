using Logistics.Application.Common;
using Logistics.Application.DTOs.Carts;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Carts.Commands.RemoveCartItem;

public class RemoveCartItemCommandHandler(IApplicationDbContext context)
    : IRequestHandler<RemoveCartItemCommand, ApiResponse<CartDto>>
{
    public async Task<ApiResponse<CartDto>> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<CartDto>.Fail("Only customer can use cart.");
        }

        var cart = await context.Carts
            .FirstOrDefaultAsync(cart => cart.UserId == request.CurrentUserId, cancellationToken);

        if (cart is null)
        {
            return ApiResponse<CartDto>.Fail("Cart does not exist.");
        }

        var cartItem = await context.CartItems
            .FirstOrDefaultAsync(
                item => item.Id == request.CartItemId && item.CartId == cart.Id,
                cancellationToken);

        if (cartItem is null)
        {
            return ApiResponse<CartDto>.Fail("Cart item does not exist.");
        }

        context.CartItems.Remove(cartItem);
        await context.SaveChangesAsync(cancellationToken);

        var cartDto = await CartDtoBuilder.BuildAsync(context, request.CurrentUserId, cancellationToken);
        return ApiResponse<CartDto>.Ok(cartDto, "Cart item removed successfully.");
    }
}
