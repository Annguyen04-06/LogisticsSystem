using Logistics.Application.Common;
using Logistics.Application.DTOs.Reports;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Reports.Queries.GetTopSellersReport;

public class GetTopSellersReportQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetTopSellersReportQuery, ApiResponse<List<TopSellerReportDto>>>
{
    public async Task<ApiResponse<List<TopSellerReportDto>>> Handle(
        GetTopSellersReportQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<List<TopSellerReportDto>>.Fail("Chỉ quản trị viên mới có quyền xem báo cáo người bán.");
        }

        var take = request.Take <= 0 ? 10 : request.Take;

        var sellers = await (
            from seller in context.Users
            where seller.Role == UserRole.Seller
            join order in context.Orders on seller.Id equals order.SellerId into sellerOrders
            select new
            {
                SellerId = seller.Id,
                SellerName = seller.FullName,
                TotalOrders = sellerOrders.Count(),
                DeliveredOrders = sellerOrders.Count(order => order.Status == OrderStatus.Delivered),
                TotalRevenue = sellerOrders
                    .Where(order => order.Status == OrderStatus.Delivered)
                    .Sum(order => (decimal?)order.FinalAmount) ?? 0
            })
            .OrderByDescending(seller => seller.TotalRevenue)
            .Take(take)
            .ToListAsync(cancellationToken);

        var deliveredOrderDetails = await (
            from order in context.Orders
            where order.Status == OrderStatus.Delivered
            join detail in context.OrderDetails on order.Id equals detail.OrderId
            group detail by order.SellerId into sellerGroup
            select new
            {
                SellerId = sellerGroup.Key,
                TotalProductsSold = sellerGroup.Sum(detail => detail.Quantity)
            })
            .ToDictionaryAsync(item => item.SellerId, item => item.TotalProductsSold, cancellationToken);

        var reports = sellers
            .Select(seller => new TopSellerReportDto
            {
                SellerId = seller.SellerId,
                SellerName = seller.SellerName,
                TotalOrders = seller.TotalOrders,
                DeliveredOrders = seller.DeliveredOrders,
                TotalProductsSold = deliveredOrderDetails.GetValueOrDefault(seller.SellerId),
                TotalRevenue = seller.TotalRevenue
            })
            .ToList();

        return ApiResponse<List<TopSellerReportDto>>.Ok(reports);
    }
}
