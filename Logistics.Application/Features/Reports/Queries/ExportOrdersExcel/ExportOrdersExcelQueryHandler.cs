using Logistics.Application.Common;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Reports.Queries.ExportOrdersExcel;

public class ExportOrdersExcelQueryHandler(IApplicationDbContext context)
    : IRequestHandler<ExportOrdersExcelQuery, ApiResponse<byte[]>>
{
    public async Task<ApiResponse<byte[]>> Handle(ExportOrdersExcelQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<byte[]>.Fail("Only admin can export orders report.");
        }

        var orders = await (
            from order in context.Orders
            join customer in context.Users on order.CustomerId equals customer.Id
            join seller in context.Users on order.SellerId equals seller.Id
            orderby order.CreatedAt descending
            select new OrderReportRow(
                order.Id,
                customer.FullName,
                seller.FullName,
                order.Status,
                order.CreatedAt,
                order.TotalAmount,
                order.DiscountAmount,
                order.FinalAmount))
            .ToListAsync(cancellationToken);

        var bytes = ReportExportBuilder.BuildOrdersExcel(orders);
        return ApiResponse<byte[]>.Ok(bytes, "Orders report exported successfully.");
    }
}
