using Logistics.Application.Common;
using Logistics.Application.DTOs.Products;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Products.Queries.GetSellerProducts;

public class GetSellerProductsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSellerProductsQuery, ApiResponse<List<ProductDto>>>
{
    public async Task<ApiResponse<List<ProductDto>>> Handle(GetSellerProductsQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Seller and not UserRole.Admin)
        {
            return ApiResponse<List<ProductDto>>.Fail("Only seller or admin can view seller products.");
        }

        var sellerId = request.CurrentUserRole == UserRole.Admin
            ? request.SellerId
            : request.CurrentUserId;

        var productsQuery = context.Products.AsQueryable();

        if (sellerId.HasValue)
        {
            productsQuery = productsQuery.Where(product => product.SellerId == sellerId.Value);
        }

        var products = await (
            from product in productsQuery
            join category in context.Categories on product.CategoryId equals category.Id
            join seller in context.Users on product.SellerId equals seller.Id
            orderby product.CreatedAt descending
            select new ProductDto
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
            }).ToListAsync(cancellationToken);

        return ApiResponse<List<ProductDto>>.Ok(products);
    }
}
