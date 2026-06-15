using Logistics.Application.Common;
using Logistics.Application.DTOs.Reports;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reports.Queries.GetAdminRevenueReport;

public class GetAdminRevenueReportQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAdminRevenueReportQuery, ApiResponse<RevenueReportDto>>
{
    public async Task<ApiResponse<RevenueReportDto>> Handle(
        GetAdminRevenueReportQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<RevenueReportDto>.Fail("Only admin can view revenue report.");
        }

        var report = await ReportQueryHelper.BuildRevenueReportAsync(context, context.Orders, cancellationToken);
        return ApiResponse<RevenueReportDto>.Ok(report);
    }
}
