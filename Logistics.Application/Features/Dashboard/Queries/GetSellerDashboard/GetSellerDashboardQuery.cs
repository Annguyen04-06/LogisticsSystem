using Logistics.Application.Common;
using Logistics.Application.DTOs.Dashboard;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Dashboard.Queries.GetSellerDashboard;

public record GetSellerDashboardQuery(
    int CurrentUserId,
    UserRole CurrentUserRole,
    int? SellerId = null) : IRequest<ApiResponse<SellerDashboardDto>>;
