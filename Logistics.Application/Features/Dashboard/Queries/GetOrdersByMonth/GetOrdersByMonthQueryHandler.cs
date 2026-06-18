using Logistics.Application.Common;
using Logistics.Application.DTOs.Dashboard;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Dashboard.Queries.GetOrdersByMonth;

public class GetOrdersByMonthQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetOrdersByMonthQuery, ApiResponse<List<OrderStatisticsByMonthDto>>>
{
    public async Task<ApiResponse<List<OrderStatisticsByMonthDto>>> Handle(
        GetOrdersByMonthQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<List<OrderStatisticsByMonthDto>>.Fail("Only admin can view order statistics by month.");
        }

        var year = request.Year ?? VietnamTime.Now.Year;

        if (year <= 0)
        {
            return ApiResponse<List<OrderStatisticsByMonthDto>>.Fail("Year is invalid.");
        }

        var startUtc = VietnamTime.ToUtc(new DateTime(year, 1, 1));
        var endUtc = VietnamTime.ToUtc(new DateTime(year + 1, 1, 1));

        var orders = await context.Orders
            .Where(order => order.CreatedAt >= startUtc && order.CreatedAt < endUtc)
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
            .Select(order => new
            {
                VietnamCreatedAt = VietnamTime.ToVietnamTime(order.CreatedAt),
                order.Status,
                order.FinalAmount,
                order.IsPaid
            })
            .GroupBy(order => new { order.VietnamCreatedAt.Year, order.VietnamCreatedAt.Month })
            .Select(group => new OrderStatisticsByMonthDto
            {
                Year = group.Key.Year,
                Month = group.Key.Month,
                TotalOrders = group.Count(),
                Revenue = group
                    .Where(order => order.Status == OrderStatus.Delivered && order.IsPaid)
                    .Sum(order => order.FinalAmount)
            })
            .OrderBy(item => item.Year)
            .ThenBy(item => item.Month)
            .ToList();

        return ApiResponse<List<OrderStatisticsByMonthDto>>.Ok(statistics);
    }
}
