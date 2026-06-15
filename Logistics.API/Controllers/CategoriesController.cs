using System.Security.Claims;
using Logistics.Application.DTOs.Categories;
using Logistics.Application.Features.Categories.Commands.CreateCategory;
using Logistics.Application.Features.Categories.Commands.DeleteCategory;
using Logistics.Application.Features.Categories.Commands.UpdateCategory;
using Logistics.Application.Features.Categories.Queries.GetAllCategories;
using Logistics.Application.Features.Categories.Queries.GetCategoryById;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetAllCategoriesQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateCategoryDto categoryDto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentRole(out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new CreateCategoryCommand(categoryDto, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateCategoryDto categoryDto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentRole(out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new UpdateCategoryCommand(id, categoryDto, currentUserRole),
            cancellationToken);

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

        var response = await mediator.Send(
            new DeleteCategoryCommand(id, currentUserRole),
            cancellationToken);

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
