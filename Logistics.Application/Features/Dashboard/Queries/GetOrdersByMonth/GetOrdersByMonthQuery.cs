using Logistics.Application.Common;
using Logistics.Application.DTOs.Dashboard;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Dashboard.Queries.GetOrdersByMonth;

public record GetOrdersByMonthQuery(
    UserRole CurrentUserRole,
    int? Year = null) : IRequest<ApiResponse<List<OrderStatisticsByMonthDto>>>;
