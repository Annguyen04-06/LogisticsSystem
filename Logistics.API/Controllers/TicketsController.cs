using System.Security.Claims;
using Logistics.Application.DTOs.Tickets;
using Logistics.Application.Features.Tickets.Commands.CloseTicket;
using Logistics.Application.Features.Tickets.Commands.CreateTicket;
using Logistics.Application.Features.Tickets.Commands.ReplyTicket;
using Logistics.Application.Features.Tickets.Commands.UpdateTicketStatus;
using Logistics.Application.Features.Tickets.Queries.GetAllTickets;
using Logistics.Application.Features.Tickets.Queries.GetMyTickets;
using Logistics.Application.Features.Tickets.Queries.GetSellerTickets;
using Logistics.Application.Features.Tickets.Queries.GetTicketById;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Create(CreateTicketDto ticketDto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new CreateTicketCommand(ticketDto, currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("my-tickets")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyTickets(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new GetMyTicketsQuery(currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("seller-tickets")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> GetSellerTickets(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new GetSellerTicketsQuery(currentUserId, currentUserRole),
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

        var response = await mediator.Send(new GetAllTicketsQuery(currentUserRole), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Customer,Seller,Admin")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new GetTicketByIdQuery(id, currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("{id:int}/reply")]
    [Authorize(Roles = "Customer,Seller,Admin")]
    public async Task<IActionResult> Reply(int id, ReplyTicketDto replyDto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new ReplyTicketCommand(id, replyDto, currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateTicketStatusDto statusDto,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new UpdateTicketStatusCommand(id, statusDto, currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("{id:int}/close")]
    [Authorize(Roles = "Customer,Seller,Admin")]
    public async Task<IActionResult> Close(int id, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new CloseTicketCommand(id, currentUserId, currentUserRole),
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
