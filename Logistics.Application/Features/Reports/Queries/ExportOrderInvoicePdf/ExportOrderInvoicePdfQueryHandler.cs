using Logistics.Application.Common;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Reports.Queries.ExportOrderInvoicePdf;

public class ExportOrderInvoicePdfQueryHandler(IApplicationDbContext context)
    : IRequestHandler<ExportOrderInvoicePdfQuery, ApiResponse<byte[]>>
{
    public async Task<ApiResponse<byte[]>> Handle(
        ExportOrderInvoicePdfQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Customer and not UserRole.Admin)
        {
            return ApiResponse<byte[]>.Fail("Only customer or admin can export invoice.");
        }

        var order = await (
            from item in context.Orders
            join customer in context.Users on item.CustomerId equals customer.Id
            join seller in context.Users on item.SellerId equals seller.Id
            where item.Id == request.OrderId
            select new
            {
                Order = item,
                CustomerName = customer.FullName,
                SellerName = seller.FullName
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (order == null)
        {
            return ApiResponse<byte[]>.Fail("Order does not exist.");
        }

        if (request.CurrentUserRole == UserRole.Customer && order.Order.CustomerId != request.CurrentUserId)
        {
            return ApiResponse<byte[]>.Fail("You can only export invoice for your own order.");
        }

        var details = await (
            from detail in context.OrderDetails
            join product in context.Products on detail.ProductId equals product.Id
            where detail.OrderId == request.OrderId
            select new OrderInvoiceDetailData(
                product.Name,
                detail.Quantity,
                detail.UnitPrice,
                detail.TotalPrice))
            .ToListAsync(cancellationToken);

        var invoice = new OrderInvoiceData(
            order.Order.Id,
            order.CustomerName,
            order.SellerName,
            order.Order.Status,
            order.Order.CreatedAt,
            order.Order.TotalAmount,
            order.Order.DiscountAmount,
            order.Order.FinalAmount,
            details);

        var bytes = ReportExportBuilder.BuildInvoicePdf(invoice);
        return ApiResponse<byte[]>.Ok(bytes, "Invoice exported successfully.");
    }
}
