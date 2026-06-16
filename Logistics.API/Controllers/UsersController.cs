using System.Security.Claims;
using Logistics.Application.DTOs.Users;
using Logistics.Application.Features.Users.Commands.ApproveSeller;
using Logistics.Application.Features.Users.Commands.DeleteUser;
using Logistics.Application.Features.Users.Commands.ToggleUserActive;
using Logistics.Application.Features.Users.Queries.GetAllUsers;
using Logistics.Application.Features.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetAllUsersQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPut("{id:int}/approve-seller")]
    public async Task<IActionResult> ApproveSeller(int id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ApproveSellerCommand(id), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("{id:int}/toggle-active")]
    public async Task<IActionResult> ToggleActive(
        int id,
        UpdateUserStatusDto updateUserStatusDto,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new ToggleUserActiveCommand(id, updateUserStatusDto.IsActive, currentUserId),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(new DeleteUserCommand(id, currentUserId), cancellationToken);

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
