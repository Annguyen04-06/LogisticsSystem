using System.Security.Claims;
using Logistics.Application.Features.Reports.Queries.ExportOrderInvoicePdf;
using Logistics.Application.Features.Reports.Queries.ExportOrdersExcel;
using Logistics.Application.Features.Reports.Queries.ExportRevenueExcel;
using Logistics.Application.Features.Reports.Queries.GetAdminRevenueReport;
using Logistics.Application.Features.Reports.Queries.GetOrderStatusStatistics;
using Logistics.Application.Features.Reports.Queries.GetSellerRevenueReport;
using Logistics.Application.Features.Reports.Queries.GetTopProductsReport;
using Logistics.Application.Features.Reports.Queries.GetTopSellersReport;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController(IMediator mediator) : ControllerBase
{
    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private const string PdfContentType = "application/pdf";

    [HttpGet("admin/revenue")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdminRevenue(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out _, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new GetAdminRevenueReportQuery(currentUserRole), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("admin/top-products")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetTopProducts(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out _, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new GetTopProductsReportQuery(currentUserRole), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("admin/top-sellers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetTopSellers(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out _, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new GetTopSellersReportQuery(currentUserRole), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("admin/order-status-statistics")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOrderStatusStatistics(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out _, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new GetOrderStatusStatisticsQuery(currentUserRole), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("admin/orders-excel")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportOrdersExcel(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out _, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new ExportOrdersExcelQuery(currentUserRole), cancellationToken);

        if (!response.Success || response.Data == null)
        {
            return BadRequest(response);
        }

        return File(response.Data, ExcelContentType, "orders-report.xlsx");
    }

    [HttpGet("admin/revenue-excel")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportAdminRevenueExcel(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new ExportRevenueExcelQuery(currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success || response.Data == null)
        {
            return BadRequest(response);
        }

        return File(response.Data, ExcelContentType, "revenue-report.xlsx");
    }

    [HttpGet("seller/revenue")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> GetSellerRevenue([FromQuery] int? sellerId, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new GetSellerRevenueReportQuery(currentUserId, currentUserRole, sellerId),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("seller/revenue-excel")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> ExportSellerRevenueExcel(
        [FromQuery] int? sellerId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new ExportRevenueExcelQuery(currentUserId, currentUserRole, sellerId),
            cancellationToken);

        if (!response.Success || response.Data == null)
        {
            return BadRequest(response);
        }

        return File(response.Data, ExcelContentType, "seller-revenue-report.xlsx");
    }

    [HttpGet("orders/{orderId:int}/invoice-pdf")]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> ExportInvoicePdf(int orderId, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new ExportOrderInvoicePdfQuery(orderId, currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success || response.Data == null)
        {
            return BadRequest(response);
        }

        return File(response.Data, PdfContentType, $"invoice-{orderId}.pdf");
    }

    private bool TryGetCurrentUser(out int userId, out UserRole role)
    {
        userId = 0;
        role = default;

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirstValue(ClaimTypes.Role);

        return int.TryParse(userIdClaim, out userId)
            && Enum.TryParse(roleClaim, out role);
    }
}
