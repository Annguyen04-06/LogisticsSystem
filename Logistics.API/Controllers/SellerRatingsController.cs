using System.Security.Claims;
using Logistics.Application.DTOs.SellerRatings;
using Logistics.Application.Features.SellerRatings.Commands.CreateSellerRating;
using Logistics.Application.Features.SellerRatings.Commands.DeleteSellerRating;
using Logistics.Application.Features.SellerRatings.Queries.GetAllSellerRatings;
using Logistics.Application.Features.SellerRatings.Queries.GetMySellerRatings;
using Logistics.Application.Features.SellerRatings.Queries.GetSellerRatings;
using Logistics.Application.Features.SellerRatings.Queries.GetSellerReputation;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/seller-ratings")]
public class SellerRatingsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Create(CreateSellerRatingDto sellerRatingDto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new CreateSellerRatingCommand(sellerRatingDto, currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("seller/{sellerId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSellerRatings(int sellerId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetSellerRatingsQuery(sellerId), cancellationToken);
        return Ok(response);
    }

    [HttpGet("my-ratings")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyRatings(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new GetMySellerRatingsQuery(currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("seller/{sellerId:int}/reputation")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSellerReputation(int sellerId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetSellerReputationQuery(sellerId), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out _, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new GetAllSellerRatingsQuery(currentUserRole), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new DeleteSellerRatingCommand(id, currentUserId, currentUserRole),
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
