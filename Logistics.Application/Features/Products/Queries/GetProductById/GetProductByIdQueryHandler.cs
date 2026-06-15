using Logistics.Application.Common;
using Logistics.Application.DTOs.Products;
using Logistics.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetProductByIdQuery, ApiResponse<ProductDto>>
{
    public async Task<ApiResponse<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var productDto = await (
            from product in context.Products
            join category in context.Categories on product.CategoryId equals category.Id
            join seller in context.Users on product.SellerId equals seller.Id
            where product.Id == request.Id && product.IsActive
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
                IsActive = product.IsActive
            }).FirstOrDefaultAsync(cancellationToken);

        if (productDto is null)
        {
            return ApiResponse<ProductDto>.Fail("Product does not exist.");
        }

        return ApiResponse<ProductDto>.Ok(productDto);
    }
}
