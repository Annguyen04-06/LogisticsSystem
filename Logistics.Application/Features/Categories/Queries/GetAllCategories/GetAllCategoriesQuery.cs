using Logistics.Application.Common;
using Logistics.Application.DTOs.Categories;
using MediatR;

namespace Logistics.Application.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQuery(bool includeInactive = false) : IRequest<ApiResponse<List<CategoryDto>>>
{
    public bool IncludeInactive { get; } = includeInactive;
}
