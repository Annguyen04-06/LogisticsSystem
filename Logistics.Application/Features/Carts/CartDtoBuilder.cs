using Logistics.Application.DTOs.Carts;
using Logistics.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Carts;

internal static class CartDtoBuilder
{
    public static async Task<CartDto> BuildAsync(
        IApplicationDbContext context,
        int customerId,
        CancellationToken cancellationToken)
    {
        var cart = await context.Carts
            .FirstOrDefaultAsync(cart => cart.UserId == customerId, cancellationToken);

        if (cart is null)
        {
            return new CartDto
            {
                CustomerId = customerId
            };
        }

        var items = await (
            from item in context.CartItems
            join product in context.Products on item.ProductId equals product.Id
            where item.CartId == cart.Id
            select new CartItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = item.Quantity,
                TotalPrice = product.Price * item.Quantity
            }).ToListAsync(cancellationToken);

        return new CartDto
        {
            CustomerId = customerId,
            Items = items,
            TotalAmount = items.Sum(item => item.TotalPrice)
        };
    }
}
