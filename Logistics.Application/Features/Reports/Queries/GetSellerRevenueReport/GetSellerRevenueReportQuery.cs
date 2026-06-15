using Logistics.Application.Common;
using Logistics.Application.DTOs.Reports;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reports.Queries.GetSellerRevenueReport;

public record GetSellerRevenueReportQuery(
    int CurrentUserId,
    UserRole CurrentUserRole,
    int? SellerId = null) : IRequest<ApiResponse<SellerRevenueReportDto>>;
