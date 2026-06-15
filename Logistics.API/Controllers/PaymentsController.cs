using System.Security.Claims;
using Logistics.Application.DTOs.Payments;
using Logistics.Application.Features.Payments.Commands.DepositWallet;
using Logistics.Application.Features.Payments.Commands.PayOrder;
using Logistics.Application.Features.Payments.Commands.RefundPayment;
using Logistics.Application.Features.Payments.Queries.GetMyPayments;
using Logistics.Application.Features.Payments.Queries.GetMyWallet;
using Logistics.Application.Features.Payments.Queries.GetPaymentByOrderId;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController(IMediator mediator) : ControllerBase
{
    [HttpGet("my-wallet")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyWallet(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new GetMyWalletQuery(currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("deposit-wallet")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> DepositWallet(DepositWalletDto depositWalletDto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new DepositWalletCommand(depositWalletDto, currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("pay-order")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> PayOrder(PayOrderDto payOrderDto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new PayOrderCommand(payOrderDto, currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("my-payments")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyPayments(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new GetMyPaymentsQuery(currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("order/{orderId:int}")]
    [Authorize(Roles = "Customer,Seller,Admin")]
    public async Task<IActionResult> GetPaymentByOrderId(int orderId, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new GetPaymentByOrderIdQuery(orderId, currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("{paymentId:int}/refund")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Refund(int paymentId, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out _, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new RefundPaymentCommand(paymentId, currentUserRole),
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
