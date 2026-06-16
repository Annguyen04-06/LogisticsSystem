using Logistics.Application.Common;
using Logistics.Application.DTOs.Reports;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reports.Queries.GetTopSellersReport;

public record GetTopSellersReportQuery(
    UserRole CurrentUserRole,
    int Take = 10) : IRequest<ApiResponse<List<TopSellerReportDto>>>;
