using Logistics.Application.DTOs.Reports;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Reports;

internal static class ReportQueryHelper
{
    public static async Task<RevenueReportDto> BuildRevenueReportAsync(
        IApplicationDbContext context,
        IQueryable<Logistics.Domain.Entities.Order> ordersQuery,
        CancellationToken cancellationToken)
    {
        return new RevenueReportDto
        {
            TotalRevenue = await ordersQuery
                .Where(order => order.Status == OrderStatus.Delivered)
                .SumAsync(order => (decimal?)order.FinalAmount, cancellationToken) ?? 0,
            TotalOrders = await ordersQuery.CountAsync(cancellationToken),
            DeliveredOrders = await ordersQuery.CountAsync(
                order => order.Status == OrderStatus.Delivered,
                cancellationToken),
            PendingOrders = await ordersQuery.CountAsync(
                order => order.Status == OrderStatus.Pending,
                cancellationToken),
            CancelledOrders = await ordersQuery.CountAsync(
                order => order.Status == OrderStatus.Cancelled,
                cancellationToken)
        };
    }

    public static async Task<SellerRevenueReportDto?> BuildSellerRevenueReportAsync(
        IApplicationDbContext context,
        int sellerId,
        CancellationToken cancellationToken)
    {
        var seller = await context.Users
            .FirstOrDefaultAsync(user => user.Id == sellerId && user.Role == UserRole.Seller, cancellationToken);

        if (seller == null)
        {
            return null;
        }

        var sellerOrders = context.Orders.Where(order => order.SellerId == sellerId);

        return new SellerRevenueReportDto
        {
            SellerId = seller.Id,
            SellerName = seller.FullName,
            TotalRevenue = await sellerOrders
                .Where(order => order.Status == OrderStatus.Delivered)
                .SumAsync(order => (decimal?)order.FinalAmount, cancellationToken) ?? 0,
            TotalOrders = await sellerOrders.CountAsync(cancellationToken),
            DeliveredOrders = await sellerOrders.CountAsync(
                order => order.Status == OrderStatus.Delivered,
                cancellationToken)
        };
    }

    public static async Task<List<SellerRevenueReportDto>> BuildSellerRevenueReportsAsync(
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        return await (
            from seller in context.Users
            where seller.Role == UserRole.Seller
            join order in context.Orders on seller.Id equals order.SellerId into sellerOrders
            select new SellerRevenueReportDto
            {
                SellerId = seller.Id,
                SellerName = seller.FullName,
                TotalRevenue = sellerOrders
                    .Where(order => order.Status == OrderStatus.Delivered)
                    .Sum(order => (decimal?)order.FinalAmount) ?? 0,
                TotalOrders = sellerOrders.Count(),
                DeliveredOrders = sellerOrders.Count(order => order.Status == OrderStatus.Delivered)
            })
            .OrderByDescending(report => report.TotalRevenue)
            .ToListAsync(cancellationToken);
    }

    public static async Task<List<TopProductReportDto>> BuildTopProductsReportAsync(
        IApplicationDbContext context,
        int? sellerId,
        CancellationToken cancellationToken)
    {
        var deliveredOrders = context.Orders.Where(order => order.Status == OrderStatus.Delivered);

        if (sellerId.HasValue)
        {
            deliveredOrders = deliveredOrders.Where(order => order.SellerId == sellerId.Value);
        }

        return await (
            from detail in context.OrderDetails
            join order in deliveredOrders on detail.OrderId equals order.Id
            join product in context.Products on detail.ProductId equals product.Id
            group new { detail, product } by new { detail.ProductId, product.Name } into productGroup
            orderby productGroup.Sum(item => item.detail.Quantity) descending
            select new TopProductReportDto
            {
                ProductId = productGroup.Key.ProductId,
                ProductName = productGroup.Key.Name,
                TotalSold = productGroup.Sum(item => item.detail.Quantity),
                TotalRevenue = productGroup.Sum(item => item.detail.TotalPrice)
            })
            .Take(20)
            .ToListAsync(cancellationToken);
    }
}
