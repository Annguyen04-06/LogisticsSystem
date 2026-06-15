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

        var year = request.Year ?? DateTime.UtcNow.Year;

        if (year <= 0)
        {
            return ApiResponse<List<OrderStatisticsByMonthDto>>.Fail("Year is invalid.");
        }

        var statistics = await context.Orders
            .Where(order => order.CreatedAt.Year == year)
            .GroupBy(order => new { order.CreatedAt.Year, order.CreatedAt.Month })
            .Select(group => new OrderStatisticsByMonthDto
            {
                Year = group.Key.Year,
                Month = group.Key.Month,
                TotalOrders = group.Count(),
                Revenue = group
                    .Where(order => order.Status == OrderStatus.Delivered)
                    .Sum(order => (decimal?)order.FinalAmount) ?? 0
            })
            .OrderBy(item => item.Year)
            .ThenBy(item => item.Month)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<OrderStatisticsByMonthDto>>.Ok(statistics);
    }
}
