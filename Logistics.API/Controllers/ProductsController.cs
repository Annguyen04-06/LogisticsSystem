using System.Security.Claims;
using Logistics.Application.DTOs.Products;
using Logistics.Application.Features.Products.Commands.CreateProduct;
using Logistics.Application.Features.Products.Commands.DeleteProduct;
using Logistics.Application.Features.Products.Commands.UpdateProduct;
using Logistics.Application.Features.Products.Queries.GetAllProducts;
using Logistics.Application.Features.Products.Queries.GetProductById;
using Logistics.Application.Features.Products.Queries.GetSellerProducts;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            new GetAllProductsQuery(search, categoryId, minPrice, maxPrice),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetProductByIdQuery(id), cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpGet("my-products")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> GetMyProducts([FromQuery] int? sellerId, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new GetSellerProductsQuery(currentUserId, currentUserRole, sellerId),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Create(CreateProductDto productDto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new CreateProductCommand(productDto, currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Update(int id, UpdateProductDto productDto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new UpdateProductCommand(id, productDto, currentUserId, currentUserRole),
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var response = await mediator.Send(
            new DeleteProductCommand(id, currentUserId, currentUserRole),
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
