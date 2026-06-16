using Logistics.Application.Common;
using Logistics.Application.DTOs.Products;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateProductCommand, ApiResponse<ProductDto>>
{
    public async Task<ApiResponse<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(product => product.Id == request.Id, cancellationToken);

        if (product is null)
        {
            return ApiResponse<ProductDto>.Fail("Product does not exist.");
        }

        if (request.CurrentUserRole != UserRole.Admin && product.SellerId != request.CurrentUserId)
        {
            return ApiResponse<ProductDto>.Fail("You cannot update another seller's product.");
        }

        var validationError = Validate(request.Product);
        if (validationError is not null)
        {
            return ApiResponse<ProductDto>.Fail(validationError);
        }

        var category = await context.Categories
            .FirstOrDefaultAsync(category => category.Id == request.Product.CategoryId && category.IsActive, cancellationToken);

        if (category is null)
        {
            return ApiResponse<ProductDto>.Fail("Category does not exist.");
        }

        product.Name = request.Product.Name.Trim();
        product.Description = request.Product.Description.Trim();
        product.Price = request.Product.Price;
        product.Quantity = request.Product.Quantity;
        product.CategoryId = request.Product.CategoryId;
        product.ImageUrl = request.Product.ImageUrl;
        product.IsActive = request.Product.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        var seller = await context.Users
            .FirstOrDefaultAsync(user => user.Id == product.SellerId, cancellationToken);

        return ApiResponse<ProductDto>.Ok(new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            CategoryId = product.CategoryId,
            CategoryName = category.Name,
            SellerId = product.SellerId,
            SellerName = seller?.FullName ?? string.Empty,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive
        }, "Product updated successfully.");
    }

    private static string? Validate(UpdateProductDto product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
        {
            return "Product name is required.";
        }

        if (product.Price <= 0)
        {
            return "Price must be greater than 0.";
        }

        if (product.Quantity < 0)
        {
            return "Quantity must be greater than or equal to 0.";
        }

        if (product.CategoryId <= 0)
        {
            return "Category is required.";
        }

        return null;
    }
}
