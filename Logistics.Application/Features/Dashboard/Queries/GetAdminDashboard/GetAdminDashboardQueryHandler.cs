using Logistics.Application.Common;
using Logistics.Application.DTOs.Dashboard;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Dashboard.Queries.GetAdminDashboard;

public class GetAdminDashboardQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAdminDashboardQuery, ApiResponse<AdminDashboardDto>>
{
    public async Task<ApiResponse<AdminDashboardDto>> Handle(
        GetAdminDashboardQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<AdminDashboardDto>.Fail("Only admin can view system dashboard.");
        }

        var dashboard = new AdminDashboardDto
        {
            TotalUsers = await context.Users.CountAsync(cancellationToken),
            TotalCustomers = await context.Users.CountAsync(
                user => user.Role == UserRole.Customer,
                cancellationToken),
            TotalSellers = await context.Users.CountAsync(
                user => user.Role == UserRole.Seller,
                cancellationToken),
            TotalShippers = await context.Users.CountAsync(
                user => user.Role == UserRole.Shipper,
                cancellationToken),
            TotalProducts = await context.Products.CountAsync(cancellationToken),
            TotalOrders = await context.Orders.CountAsync(cancellationToken),
            PendingOrders = await context.Orders.CountAsync(
                order => order.Status == OrderStatus.Pending,
                cancellationToken),
            ConfirmedOrders = await context.Orders.CountAsync(
                order => order.Status == OrderStatus.Confirmed,
                cancellationToken),
            ShippingOrders = await context.Orders.CountAsync(
                order => order.Status == OrderStatus.Shipping,
                cancellationToken),
            DeliveredOrders = await context.Orders.CountAsync(
                order => order.Status == OrderStatus.Delivered,
                cancellationToken),
            CancelledOrders = await context.Orders.CountAsync(
                order => order.Status == OrderStatus.Cancelled,
                cancellationToken),
            TotalRevenue = await context.Orders
                .Where(order => order.Status == OrderStatus.Delivered)
                .SumAsync(order => (decimal?)order.FinalAmount, cancellationToken) ?? 0,
            TotalTickets = await context.SupportTickets.CountAsync(cancellationToken),
            OpenTickets = await context.SupportTickets.CountAsync(
                ticket => ticket.Status == TicketStatus.Open,
                cancellationToken),
            ClosedTickets = await context.SupportTickets.CountAsync(
                ticket => ticket.Status == TicketStatus.Closed,
                cancellationToken)
        };

        return ApiResponse<AdminDashboardDto>.Ok(dashboard);
    }
}
