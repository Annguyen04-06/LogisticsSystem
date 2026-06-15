using Logistics.Application.Common;
using Logistics.Application.DTOs.Reports;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reports.Queries.GetTopProductsReport;

public record GetTopProductsReportQuery(
    UserRole CurrentUserRole,
    int? SellerId = null) : IRequest<ApiResponse<List<TopProductReportDto>>>;
