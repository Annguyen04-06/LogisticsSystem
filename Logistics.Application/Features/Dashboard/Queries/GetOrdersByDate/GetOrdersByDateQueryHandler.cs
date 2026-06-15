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

        var today = DateTime.UtcNow.Date;
        var fromDate = request.FromDate?.Date ?? today.AddDays(-29);
        var toDate = request.ToDate?.Date ?? today;

        if (toDate < fromDate)
        {
            return ApiResponse<List<OrderStatisticsByDateDto>>.Fail("ToDate must be greater than or equal to FromDate.");
        }

        var endExclusive = toDate.AddDays(1);

        var statistics = await context.Orders
            .Where(order => order.CreatedAt >= fromDate && order.CreatedAt < endExclusive)
            .GroupBy(order => order.CreatedAt.Date)
            .Select(group => new OrderStatisticsByDateDto
            {
                Date = group.Key,
                TotalOrders = group.Count(),
                Revenue = group
                    .Where(order => order.Status == OrderStatus.Delivered)
                    .Sum(order => (decimal?)order.FinalAmount) ?? 0
            })
            .OrderBy(item => item.Date)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<OrderStatisticsByDateDto>>.Ok(statistics);
    }
}
