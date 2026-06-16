using Logistics.Application.Common;
using Logistics.Application.DTOs.Products;
using Logistics.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllProductsQuery, ApiResponse<List<ProductDto>>>
{
    public async Task<ApiResponse<List<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var productsQuery = context.Products.Where(product => product.IsActive);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var keyword = request.Search.Trim().ToLower();
            productsQuery = productsQuery.Where(product =>
                product.Name.ToLower().Contains(keyword) ||
                product.Description.ToLower().Contains(keyword));
        }

        if (request.CategoryId.HasValue)
        {
            productsQuery = productsQuery.Where(product => product.CategoryId == request.CategoryId.Value);
        }

        if (request.MinPrice.HasValue)
        {
            productsQuery = productsQuery.Where(product => product.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            productsQuery = productsQuery.Where(product => product.Price <= request.MaxPrice.Value);
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
