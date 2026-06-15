using Logistics.Application.Common;
using Logistics.Application.DTOs.Reports;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reports.Queries.GetAdminRevenueReport;

public record GetAdminRevenueReportQuery(UserRole CurrentUserRole) : IRequest<ApiResponse<RevenueReportDto>>;
