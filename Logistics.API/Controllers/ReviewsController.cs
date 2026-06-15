using System.Security.Claims;
using Logistics.Application.DTOs.Reviews;
using Logistics.Application.Features.Reviews.Commands.CreateReview;
using Logistics.Application.Features.Reviews.Commands.DeleteReview;
using Logistics.Application.Features.Reviews.Queries.GetAllReviews;
using Logistics.Application.Features.Reviews.Queries.GetMyReviews;
using Logistics.Application.Features.Reviews.Queries.GetProductReviews;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Create(CreateReviewDto reviewDto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new CreateReviewCommand(reviewDto, currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("product/{productId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductReviews(int productId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetProductReviewsQuery(productId), cancellationToken);
        return Ok(response);
    }

    [HttpGet("my-reviews")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyReviews(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new GetMyReviewsQuery(currentUserId, currentUserRole),
            cancellationToken);

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

        var response = await mediator.Send(new GetAllReviewsQuery(currentUserRole), cancellationToken);

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
            new DeleteReviewCommand(id, currentUserId, currentUserRole),
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
