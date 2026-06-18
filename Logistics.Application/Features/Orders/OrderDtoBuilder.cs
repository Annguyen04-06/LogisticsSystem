using Logistics.Application.DTOs.Orders;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Orders;

internal static class OrderDtoBuilder
{
    public static async Task<List<OrderDto>> BuildListAsync(
        IApplicationDbContext context,
        IQueryable<Logistics.Domain.Entities.Order> ordersQuery,
        CancellationToken cancellationToken)
    {
        var orders = await (
            from order in ordersQuery
            join customer in context.Users on order.CustomerId equals customer.Id
            join seller in context.Users on order.SellerId equals seller.Id
            orderby order.CreatedAt descending
            select new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = customer.FullName,
                SellerId = order.SellerId,
                SellerName = seller.FullName,
                ShipperId = order.ShipperId,
                TotalAmount = order.TotalAmount,
                DiscountAmount = order.DiscountAmount,
                FinalAmount = order.FinalAmount,
                PaymentMethod = order.PaymentMethod,
                Status = order.Status,
                CreatedAt = order.CreatedAt
            }).ToListAsync(cancellationToken);

        if (orders.Count == 0)
        {
            return orders;
        }

        var orderIds = orders.Select(order => order.Id).ToList();
        var payments = await context.Payments
            .Where(payment => orderIds.Contains(payment.OrderId))
            .OrderByDescending(payment => payment.CreatedAt)
            .ThenByDescending(payment => payment.Id)
            .Select(payment => new
            {
                payment.OrderId,
                payment.Status,
                payment.PaidAt
            })
            .ToListAsync(cancellationToken);

        var paymentsByOrder = payments
            .GroupBy(payment => payment.OrderId)
            .ToDictionary(group => group.Key, group => group.First());

        var details = await (
            from detail in context.OrderDetails
            join product in context.Products on detail.ProductId equals product.Id
            where orderIds.Contains(detail.OrderId)
            select new
            {
                detail.OrderId,
                Detail = new OrderDetailDto
                {
                    ProductId = detail.ProductId,
                    ProductName = product.Name,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    TotalPrice = detail.TotalPrice
                }
            }).ToListAsync(cancellationToken);

        var detailsByOrder = details
            .GroupBy(detail => detail.OrderId)
            .ToDictionary(group => group.Key, group => group.Select(item => item.Detail).ToList());

        foreach (var order in orders)
        {
            if (paymentsByOrder.TryGetValue(order.Id, out var payment))
            {
                order.PaymentStatus = payment.Status;
                order.IsPaid = payment.Status == PaymentStatus.Paid;
                order.PaidAt = payment.PaidAt;
            }
            else
            {
                order.PaymentStatus = PaymentStatus.Pending;
                order.IsPaid = false;
            }

            if (detailsByOrder.TryGetValue(order.Id, out var orderDetails))
            {
                order.Details = orderDetails;
            }
        }

        return orders;
    }

    public static async Task<OrderDto?> BuildOneAsync(
        IApplicationDbContext context,
        IQueryable<Logistics.Domain.Entities.Order> ordersQuery,
        CancellationToken cancellationToken)
    {
        var orders = await BuildListAsync(context, ordersQuery, cancellationToken);
        return orders.FirstOrDefault();
    }
}
