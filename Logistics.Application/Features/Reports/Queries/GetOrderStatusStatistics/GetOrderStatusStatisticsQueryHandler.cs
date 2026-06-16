using Logistics.Application.Common;
using Logistics.Application.DTOs.Reports;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Reports.Queries.GetOrderStatusStatistics;

public class GetOrderStatusStatisticsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetOrderStatusStatisticsQuery, ApiResponse<List<OrderStatusStatisticsDto>>>
{
    public async Task<ApiResponse<List<OrderStatusStatisticsDto>>> Handle(
        GetOrderStatusStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<List<OrderStatusStatisticsDto>>.Fail("Chỉ quản trị viên mới có quyền xem thống kê đơn hàng.");
        }

        var counts = await context.Orders
            .GroupBy(order => order.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .ToDictionaryAsync(item => item.Status, item => item.Count, cancellationToken);

        var result = new List<OrderStatusStatisticsDto>
        {
            CreateStatus("Chờ xác nhận", OrderStatus.Pending, counts),
            CreateStatus("Đã xác nhận", OrderStatus.Confirmed, counts),
            CreateStatus("Đang giao", OrderStatus.Shipping, counts),
            CreateStatus("Đã giao", OrderStatus.Delivered, counts),
            CreateStatus("Đã hủy", OrderStatus.Cancelled, counts)
        };

        return ApiResponse<List<OrderStatusStatisticsDto>>.Ok(result);
    }

    private static OrderStatusStatisticsDto CreateStatus(
        string label,
        OrderStatus status,
        Dictionary<OrderStatus, int> counts)
    {
        return new OrderStatusStatisticsDto
        {
            Status = label,
            Count = counts.GetValueOrDefault(status)
        };
    }
}
