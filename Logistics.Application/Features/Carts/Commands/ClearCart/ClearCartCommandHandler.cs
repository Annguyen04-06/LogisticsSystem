using Logistics.Application.Common;
using Logistics.Application.DTOs.Carts;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Carts.Commands.ClearCart;

public class ClearCartCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ClearCartCommand, ApiResponse<CartDto>>
{
    public async Task<ApiResponse<CartDto>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<CartDto>.Fail("Only customer can use cart.");
        }

        var cart = await context.Carts
            .FirstOrDefaultAsync(cart => cart.UserId == request.CurrentUserId, cancellationToken);

        if (cart is null)
        {
            var emptyCart = await CartDtoBuilder.BuildAsync(context, request.CurrentUserId, cancellationToken);
            return ApiResponse<CartDto>.Ok(emptyCart, "Cart cleared successfully.");
        }

        var cartItems = await context.CartItems
            .Where(item => item.CartId == cart.Id)
            .ToListAsync(cancellationToken);

        context.CartItems.RemoveRange(cartItems);
        await context.SaveChangesAsync(cancellationToken);

        var cartDto = await CartDtoBuilder.BuildAsync(context, request.CurrentUserId, cancellationToken);
        return ApiResponse<CartDto>.Ok(cartDto, "Cart cleared successfully.");
    }
}
