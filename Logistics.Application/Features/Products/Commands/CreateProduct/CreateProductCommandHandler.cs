using Logistics.Application.Common;
using Logistics.Application.DTOs.Products;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateProductCommand, ApiResponse<ProductDto>>
{
    public async Task<ApiResponse<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Seller and not UserRole.Admin)
        {
            return ApiResponse<ProductDto>.Fail("Only seller or admin can create products.");
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

        var seller = await context.Users
            .FirstOrDefaultAsync(user => user.Id == request.CurrentUserId, cancellationToken);

        if (seller is null)
        {
            return ApiResponse<ProductDto>.Fail("Current user does not exist.");
        }

        var product = new Product
        {
            Name = request.Product.Name.Trim(),
            Description = request.Product.Description.Trim(),
            Price = request.Product.Price,
            Quantity = request.Product.Quantity,
            CategoryId = request.Product.CategoryId,
            SellerId = request.CurrentUserId,
            ImageUrl = request.Product.ImageUrl,
            IsActive = true
        };

        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);

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
            SellerName = seller.FullName,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive
        }, "Product created successfully.");
    }

    private static string? Validate(CreateProductDto product)
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
