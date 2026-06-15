using System.Security.Claims;
using Logistics.Application.DTOs.Deliveries;
using Logistics.Application.Features.Deliveries.Commands.AssignShipper;
using Logistics.Application.Features.Deliveries.Commands.ConfirmOrder;
using Logistics.Application.Features.Deliveries.Commands.UpdateDeliveryStatus;
using Logistics.Application.Features.Deliveries.Queries.GetDeliveryHistory;
using Logistics.Application.Features.Deliveries.Queries.GetPendingSellerOrders;
using Logistics.Application.Features.Deliveries.Queries.GetShipperAssignedOrders;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/deliveries")]
public class DeliveriesController(IMediator mediator) : ControllerBase
{
    [HttpPut("orders/{orderId:int}/confirm")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> ConfirmOrder(int orderId, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new ConfirmOrderCommand(orderId, currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("orders/{orderId:int}/assign-shipper")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> AssignShipper(
        int orderId,
        AssignShipperDto assignShipperDto,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new AssignShipperCommand(orderId, assignShipperDto, currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("{deliveryId:int}/status")]
    [Authorize(Roles = "Shipper,Admin")]
    public async Task<IActionResult> UpdateStatus(
        int deliveryId,
        UpdateDeliveryStatusDto updateDeliveryStatusDto,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new UpdateDeliveryStatusCommand(deliveryId, updateDeliveryStatusDto, currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("seller-orders")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> GetSellerOrders(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new GetPendingSellerOrdersQuery(currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("shipper-orders")]
    [Authorize(Roles = "Shipper,Admin")]
    public async Task<IActionResult> GetShipperOrders(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new GetShipperAssignedOrdersQuery(currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("history")]
    [Authorize(Roles = "Shipper,Admin")]
    public async Task<IActionResult> GetHistory(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new GetDeliveryHistoryQuery(currentUserId, currentUserRole),
            cancellationToken);

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
