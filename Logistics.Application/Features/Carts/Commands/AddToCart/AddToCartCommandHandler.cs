using Logistics.Application.Common;
using Logistics.Application.DTOs.Carts;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Carts.Commands.AddToCart;

public class AddToCartCommandHandler(IApplicationDbContext context)
    : IRequestHandler<AddToCartCommand, ApiResponse<CartDto>>
{
    public async Task<ApiResponse<CartDto>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<CartDto>.Fail("Only customer can use cart.");
        }

        if (request.CartItem.Quantity <= 0)
        {
            return ApiResponse<CartDto>.Fail("Quantity must be greater than 0.");
        }

        var product = await context.Products
            .FirstOrDefaultAsync(product => product.Id == request.CartItem.ProductId, cancellationToken);

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
            return ApiResponse<CartDto>.Fail("Quantity cannot exceed product stock.");
        }

        var cart = await context.Carts
            .FirstOrDefaultAsync(cart => cart.UserId == request.CurrentUserId, cancellationToken);

        if (cart is null)
        {
            cart = new Cart
            {
                UserId = request.CurrentUserId
            };

            context.Carts.Add(cart);
            await context.SaveChangesAsync(cancellationToken);
        }

        var existingItem = await context.CartItems
            .FirstOrDefaultAsync(
                item => item.CartId == cart.Id && item.ProductId == product.Id,
                cancellationToken);

        if (existingItem is not null)
        {
            var newQuantity = existingItem.Quantity + request.CartItem.Quantity;

            if (newQuantity > product.Quantity)
            {
                return ApiResponse<CartDto>.Fail("Quantity cannot exceed product stock.");
            }

            existingItem.Quantity = newQuantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            context.CartItems.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = request.CartItem.Quantity
            });
        }

        await context.SaveChangesAsync(cancellationToken);

        var cartDto = await CartDtoBuilder.BuildAsync(context, request.CurrentUserId, cancellationToken);
        return ApiResponse<CartDto>.Ok(cartDto, "Product added to cart successfully.");
    }
}
