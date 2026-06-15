using System.Security.Claims;
using Logistics.Application.Features.Dashboard.Queries.GetAdminDashboard;
using Logistics.Application.Features.Dashboard.Queries.GetOrdersByDate;
using Logistics.Application.Features.Dashboard.Queries.GetOrdersByMonth;
using Logistics.Application.Features.Dashboard.Queries.GetSellerDashboard;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdminDashboard(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out _, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new GetAdminDashboardQuery(currentUserRole), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("seller")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> GetSellerDashboard(
        [FromQuery] int? sellerId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new GetSellerDashboardQuery(currentUserId, currentUserRole, sellerId),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("orders-by-date")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOrdersByDate(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out _, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new GetOrdersByDateQuery(currentUserRole, fromDate, toDate),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("orders-by-month")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOrdersByMonth(
        [FromQuery] int? year,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out _, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new GetOrdersByMonthQuery(currentUserRole, year), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
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
