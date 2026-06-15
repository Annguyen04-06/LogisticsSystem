using Logistics.Application.Common;
using Logistics.Application.DTOs.Products;
using MediatR;

namespace Logistics.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ApiResponse<ProductDto>>
{
    public int Id { get; }

    public GetProductByIdQuery(int id)
    {
        Id = id;
    }
}
