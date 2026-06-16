using System.Security.Claims;
using Logistics.Application.DTOs.Profile;
using Logistics.Application.Features.Profile.Commands.ChangePassword;
using Logistics.Application.Features.Profile.Commands.UpdateMyProfile;
using Logistics.Application.Features.Profile.Queries.GetMyProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize(Roles = "Customer,Seller,Shipper,Admin")]
public class ProfileController(IMediator mediator) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new GetMyProfileQuery(currentUserId), cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateProfileDto profileDto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new UpdateMyProfileCommand(currentUserId, profileDto), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto passwordDto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new ChangePasswordCommand(currentUserId, passwordDto), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        userId = 0;
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out userId);
    }
}
