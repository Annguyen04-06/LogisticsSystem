using Logistics.Application.Common;
using Logistics.Application.DTOs.Reports;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reports.Queries.GetTopProductsReport;

public class GetTopProductsReportQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetTopProductsReportQuery, ApiResponse<List<TopProductReportDto>>>
{
    public async Task<ApiResponse<List<TopProductReportDto>>> Handle(
        GetTopProductsReportQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<List<TopProductReportDto>>.Fail("Only admin can view top products report.");
        }

        var products = await ReportQueryHelper.BuildTopProductsReportAsync(
            context,
            request.SellerId,
            cancellationToken);

        return ApiResponse<List<TopProductReportDto>>.Ok(products);
    }
}
