using Logistics.Application.Common;
using Logistics.Application.DTOs.Reports;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reports.Queries.GetOrderStatusStatistics;

public record GetOrderStatusStatisticsQuery(UserRole CurrentUserRole)
    : IRequest<ApiResponse<List<OrderStatusStatisticsDto>>>;
