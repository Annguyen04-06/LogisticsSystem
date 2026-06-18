using Logistics.Application.Common;
using Logistics.Application.DTOs.Carts;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Carts.Commands.UpdateCartItem;

public class UpdateCartItemCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateCartItemCommand, ApiResponse<CartDto>>
{
    public async Task<ApiResponse<CartDto>> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<CartDto>.Fail("Only customer can use cart.");
        }

        if (request.CartItem.Quantity <= 0)
        {
            return ApiResponse<CartDto>.Fail("Số lượng không hợp lệ.");
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

        var product = await context.Products
            .FirstOrDefaultAsync(product => product.Id == cartItem.ProductId, cancellationToken);

        if (product is null)
        {
            return ApiResponse<CartDto>.Fail("Product does not exist.");
        }

        if (!product.IsActive)
        {
            return ApiResponse<CartDto>.Fail("Product is not active.");
        }

        if (request.CartItem.Quantity > product.Quantity)
        {
            return ApiResponse<CartDto>.Fail("Số lượng không được vượt quá tồn kho sản phẩm.");
        }

        cartItem.Quantity = request.CartItem.Quantity;
        cartItem.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        var cartDto = await CartDtoBuilder.BuildAsync(context, request.CurrentUserId, cancellationToken);
        return ApiResponse<CartDto>.Ok(cartDto, "Cập nhật giỏ hàng thành công.");
    }
}
