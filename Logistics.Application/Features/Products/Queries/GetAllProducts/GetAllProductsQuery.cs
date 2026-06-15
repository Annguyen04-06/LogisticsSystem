using Logistics.Application.Common;
using Logistics.Application.DTOs.Products;
using MediatR;

namespace Logistics.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQuery : IRequest<ApiResponse<List<ProductDto>>>
{
    public string? Search { get; }
    public int? CategoryId { get; }
    public decimal? MinPrice { get; }
    public decimal? MaxPrice { get; }

    public GetAllProductsQuery(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice)
    {
        Search = search;
        CategoryId = categoryId;
        MinPrice = minPrice;
        MaxPrice = maxPrice;
    }
}
