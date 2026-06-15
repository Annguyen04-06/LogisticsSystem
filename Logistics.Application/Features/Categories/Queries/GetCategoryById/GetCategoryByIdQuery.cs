using Logistics.Application.Common;
using Logistics.Application.DTOs.Categories;
using MediatR;

namespace Logistics.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQuery : IRequest<ApiResponse<CategoryDto>>
{
    public int Id { get; }

    public GetCategoryByIdQuery(int id)
    {
        Id = id;
    }
}
