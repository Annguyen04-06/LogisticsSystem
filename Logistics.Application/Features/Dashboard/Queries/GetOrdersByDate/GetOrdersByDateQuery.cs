using Logistics.Application.Common;
using Logistics.Application.DTOs.Dashboard;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Dashboard.Queries.GetOrdersByDate;

public record GetOrdersByDateQuery(
    UserRole CurrentUserRole,
    DateTime? FromDate = null,
    DateTime? ToDate = null) : IRequest<ApiResponse<List<OrderStatisticsByDateDto>>>;
