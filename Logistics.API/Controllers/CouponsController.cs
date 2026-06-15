using System.Security.Claims;
using Logistics.Application.DTOs.Coupons;
using Logistics.Application.Features.Coupons.Commands.ApplyCoupon;
using Logistics.Application.Features.Coupons.Commands.CreateCoupon;
using Logistics.Application.Features.Coupons.Commands.DeleteCoupon;
using Logistics.Application.Features.Coupons.Commands.UpdateCoupon;
using Logistics.Application.Features.Coupons.Queries.GetAllCoupons;
using Logistics.Application.Features.Coupons.Queries.GetCouponById;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/coupons")]
public class CouponsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentRole(out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new GetAllCouponsQuery(currentUserRole), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentRole(out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new GetCouponByIdQuery(id, currentUserRole), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateCouponDto couponDto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentRole(out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new CreateCouponCommand(couponDto, currentUserRole), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateCouponDto couponDto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentRole(out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new UpdateCouponCommand(id, couponDto, currentUserRole), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentRole(out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new DeleteCouponCommand(id, currentUserRole), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("apply")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Apply(ApplyCouponDto couponDto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentRole(out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new ApplyCouponCommand(couponDto, currentUserRole), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    private bool TryGetCurrentRole(out UserRole role)
    {
        role = default;
        var roleClaim = User.FindFirstValue(ClaimTypes.Role);

        return Enum.TryParse(roleClaim, out role);
    }
}
