using Logistics.Application.Common;
using Logistics.Application.DTOs.Orders;
using Logistics.Application.Features.Coupons;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateOrderCommand, ApiResponse<List<OrderDto>>>
{
    public async Task<ApiResponse<List<OrderDto>>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<List<OrderDto>>.Fail("Only customer can create orders.");
        }

        if (string.IsNullOrWhiteSpace(request.Order.ShippingAddress))
        {
            return ApiResponse<List<OrderDto>>.Fail("Shipping address is required.");
        }

        var cart = await context.Carts
            .FirstOrDefaultAsync(cart => cart.UserId == request.CurrentUserId, cancellationToken);

        if (cart is null)
        {
            return ApiResponse<List<OrderDto>>.Fail("Cart is empty.");
        }

        var cartItems = await context.CartItems
            .Where(item => item.CartId == cart.Id)
            .ToListAsync(cancellationToken);

        if (cartItems.Count == 0)
        {
            return ApiResponse<List<OrderDto>>.Fail("Cart is empty.");
        }

        var productIds = cartItems.Select(item => item.ProductId).ToList();
        var products = await context.Products
            .Where(product => productIds.Contains(product.Id))
            .ToListAsync(cancellationToken);

        foreach (var cartItem in cartItems)
        {
            var product = products.FirstOrDefault(product => product.Id == cartItem.ProductId);

            if (product is null)
            {
                return ApiResponse<List<OrderDto>>.Fail("Product does not exist.");
            }

            if (!product.IsActive)
            {
                return ApiResponse<List<OrderDto>>.Fail($"Product '{product.Name}' is not active.");
            }

            if (cartItem.Quantity > product.Quantity)
            {
                return ApiResponse<List<OrderDto>>.Fail($"Product '{product.Name}' does not have enough stock.");
            }
        }

        var joinedItems = cartItems
            .Join(
                products,
                item => item.ProductId,
                product => product.Id,
                (item, product) => new { CartItem = item, Product = product })
            .ToList();

        var totalCartAmount = joinedItems.Sum(item => item.Product.Price * item.CartItem.Quantity);
        Coupon? coupon = null;
        var totalDiscountAmount = 0m;

        if (!string.IsNullOrWhiteSpace(request.Order.CouponCode))
        {
            var code = request.Order.CouponCode.Trim().ToUpper();
            coupon = await context.Coupons
                .FirstOrDefaultAsync(coupon => coupon.Code.ToUpper() == code, cancellationToken);

            if (coupon is null)
            {
                return ApiResponse<List<OrderDto>>.Fail("Coupon does not exist.");
            }

            var couponError = CouponHelper.ValidateCanApply(coupon, totalCartAmount, DateTime.UtcNow);
            if (couponError is not null)
            {
                return ApiResponse<List<OrderDto>>.Fail(couponError);
            }

            totalDiscountAmount = CouponHelper.CalculateDiscount(coupon, totalCartAmount);
        }

        var createdOrderIds = new List<int>();
        var isFirstOrder = true;
        var itemsBySeller = joinedItems.GroupBy(item => item.Product.SellerId);

        foreach (var sellerGroup in itemsBySeller)
        {
            var totalAmount = sellerGroup.Sum(item => item.Product.Price * item.CartItem.Quantity);
            var discountAmount = isFirstOrder ? totalDiscountAmount : 0;
            var order = new Order
            {
                CustomerId = request.CurrentUserId,
                SellerId = sellerGroup.Key,
                TotalAmount = totalAmount,
                DiscountAmount = discountAmount,
                FinalAmount = totalAmount - discountAmount,
                Status = OrderStatus.Pending
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync(cancellationToken);
            createdOrderIds.Add(order.Id);

            if (isFirstOrder && coupon is not null)
            {
                coupon.UsedCount += 1;
                coupon.UpdatedAt = DateTime.UtcNow;

                context.CouponUsages.Add(new CouponUsage
                {
                    CouponId = coupon.Id,
                    UserId = request.CurrentUserId,
                    OrderId = order.Id,
                    DiscountAmount = totalDiscountAmount
                });
            }

            foreach (var item in sellerGroup)
            {
                context.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.Product.Id,
                    Quantity = item.CartItem.Quantity,
                    UnitPrice = item.Product.Price,
                    TotalPrice = item.Product.Price * item.CartItem.Quantity
                });

                item.Product.Quantity -= item.CartItem.Quantity;
                item.Product.UpdatedAt = DateTime.UtcNow;

                context.StockTransactions.Add(new StockTransaction
                {
                    ProductId = item.Product.Id,
                    Quantity = item.CartItem.Quantity,
                    Type = "Export",
                    Note = "Order created"
                });
            }

            isFirstOrder = false;
        }

        context.CartItems.RemoveRange(cartItems);
        await context.SaveChangesAsync(cancellationToken);

        var orders = await OrderDtoBuilder.BuildListAsync(
            context,
            context.Orders.Where(order => createdOrderIds.Contains(order.Id)),
            cancellationToken);

        return ApiResponse<List<OrderDto>>.Ok(orders, "Order created successfully.");
    }
}
