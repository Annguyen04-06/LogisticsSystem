using ClosedXML.Excel;
using Logistics.Application.Common;
using Logistics.Application.DTOs.Reports;
using Logistics.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Logistics.Application.Features.Reports;

internal static class ReportExportBuilder
{
    public static byte[] BuildOrdersExcel(IReadOnlyList<OrderReportRow> orders)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Orders");

        AddTitle(worksheet, "Orders Report", 8);
        AddHeader(worksheet, 3, "Order Id", "Customer", "Seller", "Status", "Created At", "Total", "Discount", "Final");

        var row = 4;
        foreach (var order in orders)
        {
            worksheet.Cell(row, 1).Value = order.OrderId;
            worksheet.Cell(row, 2).Value = order.CustomerName;
            worksheet.Cell(row, 3).Value = order.SellerName;
            worksheet.Cell(row, 4).Value = order.Status.ToString();
            worksheet.Cell(row, 5).Value = VietnamTime.ToVietnamTime(order.CreatedAt);
            worksheet.Cell(row, 6).Value = order.TotalAmount;
            worksheet.Cell(row, 7).Value = order.DiscountAmount;
            worksheet.Cell(row, 8).Value = order.FinalAmount;
            row++;
        }

        worksheet.Column(5).Style.DateFormat.Format = "yyyy-mm-dd hh:mm";
        worksheet.Columns(6, 8).Style.NumberFormat.Format = "#,##0.00";
        FormatWorksheet(worksheet);
        return SaveWorkbook(workbook);
    }

    public static byte[] BuildRevenueExcel(
        RevenueReportDto revenue,
        IReadOnlyList<SellerRevenueReportDto> sellerReports,
        IReadOnlyList<TopProductReportDto> topProducts)
    {
        using var workbook = new XLWorkbook();

        var summary = workbook.Worksheets.Add("Revenue");
        AddTitle(summary, "Revenue Report", 2);
        summary.Cell(3, 1).Value = "Total Revenue";
        summary.Cell(3, 2).Value = revenue.TotalRevenue;
        summary.Cell(4, 1).Value = "Total Orders";
        summary.Cell(4, 2).Value = revenue.TotalOrders;
        summary.Cell(5, 1).Value = "Delivered Orders";
        summary.Cell(5, 2).Value = revenue.DeliveredOrders;
        summary.Cell(6, 1).Value = "Pending Orders";
        summary.Cell(6, 2).Value = revenue.PendingOrders;
        summary.Cell(7, 1).Value = "Cancelled Orders";
        summary.Cell(7, 2).Value = revenue.CancelledOrders;
        summary.Cell(3, 2).Style.NumberFormat.Format = "#,##0.00";
        FormatWorksheet(summary);

        var sellers = workbook.Worksheets.Add("Seller Revenue");
        AddTitle(sellers, "Seller Revenue", 4);
        AddHeader(sellers, 3, "Seller Id", "Seller Name", "Total Revenue", "Total Orders");
        var row = 4;
        foreach (var seller in sellerReports)
        {
            sellers.Cell(row, 1).Value = seller.SellerId;
            sellers.Cell(row, 2).Value = seller.SellerName;
            sellers.Cell(row, 3).Value = seller.TotalRevenue;
            sellers.Cell(row, 4).Value = seller.TotalOrders;
            row++;
        }

        sellers.Column(3).Style.NumberFormat.Format = "#,##0.00";
        FormatWorksheet(sellers);
        AddTopProductsSheet(workbook, topProducts);

        return SaveWorkbook(workbook);
    }

    public static byte[] BuildSellerRevenueExcel(
        SellerRevenueReportDto sellerReport,
        IReadOnlyList<TopProductReportDto> topProducts)
    {
        using var workbook = new XLWorkbook();
        var summary = workbook.Worksheets.Add("Seller Revenue");

        AddTitle(summary, "Seller Revenue Report", 2);
        summary.Cell(3, 1).Value = "Seller Id";
        summary.Cell(3, 2).Value = sellerReport.SellerId;
        summary.Cell(4, 1).Value = "Seller Name";
        summary.Cell(4, 2).Value = sellerReport.SellerName;
        summary.Cell(5, 1).Value = "Total Revenue";
        summary.Cell(5, 2).Value = sellerReport.TotalRevenue;
        summary.Cell(6, 1).Value = "Total Orders";
        summary.Cell(6, 2).Value = sellerReport.TotalOrders;
        summary.Cell(5, 2).Style.NumberFormat.Format = "#,##0.00";
        FormatWorksheet(summary);

        AddTopProductsSheet(workbook, topProducts);
        return SaveWorkbook(workbook);
    }

    public static byte[] BuildInvoicePdf(OrderInvoiceData invoice)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(32);
                page.Header().Text($"Invoice #{invoice.OrderId}")
                    .FontSize(22)
                    .Bold();

                page.Content().PaddingTop(20).Column(column =>
                {
                    column.Spacing(14);

                    column.Item().Text($"Customer: {invoice.CustomerName}");
                    column.Item().Text($"Seller: {invoice.SellerName}");
                    column.Item().Text($"Status: {invoice.Status}");
                    column.Item().Text($"Ngày tạo: {VietnamTime.ToVietnamTime(invoice.CreatedAt):dd/MM/yyyy HH:mm}");

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4);
                            columns.ConstantColumn(70);
                            columns.ConstantColumn(90);
                            columns.ConstantColumn(90);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCell).Text("Product");
                            header.Cell().Element(HeaderCell).AlignRight().Text("Qty");
                            header.Cell().Element(HeaderCell).AlignRight().Text("Unit Price");
                            header.Cell().Element(HeaderCell).AlignRight().Text("Total");
                        });

                        foreach (var detail in invoice.Details)
                        {
                            table.Cell().Element(Cell).Text(detail.ProductName);
                            table.Cell().Element(Cell).AlignRight().Text(detail.Quantity.ToString());
                            table.Cell().Element(Cell).AlignRight().Text($"{detail.UnitPrice:n0}");
                            table.Cell().Element(Cell).AlignRight().Text($"{detail.TotalPrice:n0}");
                        }
                    });

                    column.Item().AlignRight().Column(totals =>
                    {
                        totals.Item().Text($"Total Amount: {invoice.TotalAmount:n0}");
                        totals.Item().Text($"Discount Amount: {invoice.DiscountAmount:n0}");
                        totals.Item().Text($"Final Amount: {invoice.FinalAmount:n0}").Bold();
                    });
                });

                page.Footer().AlignCenter().Text("Logistics Management System");
            });
        }).GeneratePdf();
    }

    private static void AddTopProductsSheet(XLWorkbook workbook, IReadOnlyList<TopProductReportDto> topProducts)
    {
        var worksheet = workbook.Worksheets.Add("Top Products");
        AddTitle(worksheet, "Top Products", 4);
        AddHeader(worksheet, 3, "Product Id", "Product Name", "Total Sold", "Total Revenue");

        var row = 4;
        foreach (var product in topProducts)
        {
            worksheet.Cell(row, 1).Value = product.ProductId;
            worksheet.Cell(row, 2).Value = product.ProductName;
            worksheet.Cell(row, 3).Value = product.TotalSold;
            worksheet.Cell(row, 4).Value = product.TotalRevenue;
            row++;
        }

        worksheet.Column(4).Style.NumberFormat.Format = "#,##0.00";
        FormatWorksheet(worksheet);
    }

    private static void AddTitle(IXLWorksheet worksheet, string title, int lastColumn)
    {
        worksheet.Cell(1, 1).Value = title;
        worksheet.Range(1, 1, 1, lastColumn).Merge();
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
    }

    private static void AddHeader(IXLWorksheet worksheet, int row, params string[] headers)
    {
        for (var index = 0; index < headers.Length; index++)
        {
            worksheet.Cell(row, index + 1).Value = headers[index];
        }

        var range = worksheet.Range(row, 1, row, headers.Length);
        range.Style.Font.Bold = true;
        range.Style.Fill.BackgroundColor = XLColor.LightGray;
    }

    private static void FormatWorksheet(IXLWorksheet worksheet)
    {
        worksheet.Columns().AdjustToContents();
        worksheet.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
    }

    private static byte[] SaveWorkbook(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static IContainer HeaderCell(IContainer container)
    {
        return container
            .DefaultTextStyle(text => text.Bold())
            .BorderBottom(1)
            .PaddingVertical(5);
    }

    private static IContainer Cell(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2)
            .PaddingVertical(4);
    }
}

internal record OrderReportRow(
    int OrderId,
    string CustomerName,
    string SellerName,
    OrderStatus Status,
    DateTime CreatedAt,
    decimal TotalAmount,
    decimal DiscountAmount,
    decimal FinalAmount);

internal record OrderInvoiceData(
    int OrderId,
    string CustomerName,
    string SellerName,
    OrderStatus Status,
    DateTime CreatedAt,
    decimal TotalAmount,
    decimal DiscountAmount,
    decimal FinalAmount,
    IReadOnlyList<OrderInvoiceDetailData> Details);

internal record OrderInvoiceDetailData(
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);
