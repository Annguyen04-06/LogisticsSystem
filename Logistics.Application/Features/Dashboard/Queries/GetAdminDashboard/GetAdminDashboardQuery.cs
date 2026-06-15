using Logistics.Application.Common;
using Logistics.Application.DTOs.Dashboard;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Dashboard.Queries.GetAdminDashboard;

public record GetAdminDashboardQuery(UserRole CurrentUserRole) : IRequest<ApiResponse<AdminDashboardDto>>;
