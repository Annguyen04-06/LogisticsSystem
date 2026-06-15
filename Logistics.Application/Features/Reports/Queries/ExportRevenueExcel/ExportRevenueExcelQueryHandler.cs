using Logistics.Application.Common;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reports.Queries.ExportRevenueExcel;

public class ExportRevenueExcelQueryHandler(IApplicationDbContext context)
    : IRequestHandler<ExportRevenueExcelQuery, ApiResponse<byte[]>>
{
    public async Task<ApiResponse<byte[]>> Handle(ExportRevenueExcelQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Admin and not UserRole.Seller)
        {
            return ApiResponse<byte[]>.Fail("Only admin or seller can export revenue report.");
        }

        if (request.CurrentUserRole == UserRole.Seller)
        {
            return await ExportSellerRevenueAsync(context, request.CurrentUserId, cancellationToken);
        }

        if (request.SellerId.HasValue)
        {
            return await ExportSellerRevenueAsync(context, request.SellerId.Value, cancellationToken);
        }

        var revenue = await ReportQueryHelper.BuildRevenueReportAsync(context, context.Orders, cancellationToken);
        var sellers = await ReportQueryHelper.BuildSellerRevenueReportsAsync(context, cancellationToken);
        var topProducts = await ReportQueryHelper.BuildTopProductsReportAsync(context, null, cancellationToken);
        var bytes = ReportExportBuilder.BuildRevenueExcel(revenue, sellers, topProducts);

        return ApiResponse<byte[]>.Ok(bytes, "Revenue report exported successfully.");
    }

    private static async Task<ApiResponse<byte[]>> ExportSellerRevenueAsync(
        IApplicationDbContext context,
        int sellerId,
        CancellationToken cancellationToken)
    {
        var sellerReport = await ReportQueryHelper.BuildSellerRevenueReportAsync(context, sellerId, cancellationToken);

        if (sellerReport == null)
        {
            return ApiResponse<byte[]>.Fail("Seller does not exist.");
        }

        var topProducts = await ReportQueryHelper.BuildTopProductsReportAsync(context, sellerId, cancellationToken);
        var bytes = ReportExportBuilder.BuildSellerRevenueExcel(sellerReport, topProducts);

        return ApiResponse<byte[]>.Ok(bytes, "Seller revenue report exported successfully.");
    }
}
