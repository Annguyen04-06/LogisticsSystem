using Logistics.Application.Common;
using Logistics.Application.DTOs.Reports;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reports.Queries.GetSellerRevenueReport;

public class GetSellerRevenueReportQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSellerRevenueReportQuery, ApiResponse<SellerRevenueReportDto>>
{
    public async Task<ApiResponse<SellerRevenueReportDto>> Handle(
        GetSellerRevenueReportQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Seller and not UserRole.Admin)
        {
            return ApiResponse<SellerRevenueReportDto>.Fail("Only seller or admin can view seller revenue report.");
        }

        if (request.CurrentUserRole == UserRole.Admin && !request.SellerId.HasValue)
        {
            return ApiResponse<SellerRevenueReportDto>.Fail("SellerId is required for admin seller revenue report.");
        }

        var sellerId = request.CurrentUserRole == UserRole.Seller
            ? request.CurrentUserId
            : request.SellerId!.Value;

        var report = await ReportQueryHelper.BuildSellerRevenueReportAsync(context, sellerId, cancellationToken);

        if (report == null)
        {
            return ApiResponse<SellerRevenueReportDto>.Fail("Seller does not exist.");
        }

        return ApiResponse<SellerRevenueReportDto>.Ok(report);
    }
}
