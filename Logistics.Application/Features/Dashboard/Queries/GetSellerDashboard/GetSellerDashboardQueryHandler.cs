using Logistics.Application.Common;
using Logistics.Application.DTOs.Dashboard;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Dashboard.Queries.GetSellerDashboard;

public class GetSellerDashboardQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSellerDashboardQuery, ApiResponse<SellerDashboardDto>>
{
    public async Task<ApiResponse<SellerDashboardDto>> Handle(
        GetSellerDashboardQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Seller and not UserRole.Admin)
        {
            return ApiResponse<SellerDashboardDto>.Fail("Only seller or admin can view seller dashboard.");
        }

        if (request.CurrentUserRole == UserRole.Admin && !request.SellerId.HasValue)
        {
            return ApiResponse<SellerDashboardDto>.Fail("SellerId is required for admin seller dashboard.");
        }

        var sellerId = request.CurrentUserRole == UserRole.Seller
            ? request.CurrentUserId
            : request.SellerId!.Value;

        var seller = await context.Users
            .FirstOrDefaultAsync(
                user => user.Id == sellerId && user.Role == UserRole.Seller,
                cancellationToken);

        if (seller == null)
        {
            return ApiResponse<SellerDashboardDto>.Fail("Seller does not exist.");
        }

        var sellerOrders = context.Orders.Where(order => order.SellerId == sellerId);
        var totalLikes = await context.SellerRatings.CountAsync(
            rating => rating.SellerId == sellerId && rating.IsLike,
            cancellationToken);
        var totalDislikes = await context.SellerRatings.CountAsync(
            rating => rating.SellerId == sellerId && !rating.IsLike,
            cancellationToken);
        var totalRatings = totalLikes + totalDislikes;

        var dashboard = new SellerDashboardDto
        {
            SellerId = seller.Id,
            SellerName = seller.FullName,
            TotalProducts = await context.Products.CountAsync(
                product => product.SellerId == sellerId,
                cancellationToken),
            TotalOrders = await sellerOrders.CountAsync(cancellationToken),
            PendingOrders = await sellerOrders.CountAsync(
                order => order.Status == OrderStatus.Pending,
                cancellationToken),
            ConfirmedOrders = await sellerOrders.CountAsync(
                order => order.Status == OrderStatus.Confirmed,
                cancellationToken),
            ShippingOrders = await sellerOrders.CountAsync(
                order => order.Status == OrderStatus.Shipping,
                cancellationToken),
            DeliveredOrders = await sellerOrders.CountAsync(
                order => order.Status == OrderStatus.Delivered,
                cancellationToken),
            CancelledOrders = await sellerOrders.CountAsync(
                order => order.Status == OrderStatus.Cancelled,
                cancellationToken),
            TotalRevenue = await sellerOrders
                .Where(order =>
                    order.Status == OrderStatus.Delivered &&
                    context.Payments.Any(payment =>
                        payment.OrderId == order.Id && payment.Status == PaymentStatus.Paid) &&
                    !context.Payments.Any(payment =>
                        payment.OrderId == order.Id && payment.Status == PaymentStatus.Refunded))
                .SumAsync(order => (decimal?)order.FinalAmount, cancellationToken) ?? 0,
            TotalLikes = totalLikes,
            TotalDislikes = totalDislikes,
            TrustPercent = totalRatings == 0 ? 0 : (decimal)totalLikes / totalRatings * 100
        };

        return ApiResponse<SellerDashboardDto>.Ok(dashboard);
    }
}
