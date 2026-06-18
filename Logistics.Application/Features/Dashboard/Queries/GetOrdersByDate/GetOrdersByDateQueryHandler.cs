using Logistics.Application.Common;
using Logistics.Application.DTOs.Dashboard;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Dashboard.Queries.GetOrdersByDate;

public class GetOrdersByDateQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetOrdersByDateQuery, ApiResponse<List<OrderStatisticsByDateDto>>>
{
    public async Task<ApiResponse<List<OrderStatisticsByDateDto>>> Handle(
        GetOrdersByDateQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<List<OrderStatisticsByDateDto>>.Fail("Only admin can view order statistics by date.");
        }

        var today = VietnamTime.Today;
        var fromDate = request.FromDate?.Date ?? today.AddDays(-29);
        var toDate = request.ToDate?.Date ?? today;

        if (toDate < fromDate)
        {
            return ApiResponse<List<OrderStatisticsByDateDto>>.Fail("ToDate must be greater than or equal to FromDate.");
        }

        var startUtc = VietnamTime.ToUtc(fromDate);
        var endExclusiveUtc = VietnamTime.ToUtc(toDate.AddDays(1));

        var orders = await context.Orders
            .Where(order => order.CreatedAt >= startUtc && order.CreatedAt < endExclusiveUtc)
            .Select(order => new
            {
                order.CreatedAt,
                order.Status,
                order.FinalAmount,
                IsPaid = context.Payments.Any(payment =>
                    payment.OrderId == order.Id && payment.Status == PaymentStatus.Paid) &&
                    !context.Payments.Any(payment =>
                        payment.OrderId == order.Id && payment.Status == PaymentStatus.Refunded)
            })
            .ToListAsync(cancellationToken);

        var statistics = orders
            .GroupBy(order => VietnamTime.ToVietnamTime(order.CreatedAt).Date)
            .Select(group => new OrderStatisticsByDateDto
            {
                Date = group.Key,
                TotalOrders = group.Count(),
                Revenue = group
                    .Where(order => order.Status == OrderStatus.Delivered && order.IsPaid)
                    .Sum(order => order.FinalAmount)
            })
            .OrderBy(item => item.Date)
            .ToList();

        return ApiResponse<List<OrderStatisticsByDateDto>>.Ok(statistics);
    }
}
