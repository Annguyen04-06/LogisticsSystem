using Logistics.Application.Common;
using Logistics.Application.DTOs.Categories;
using MediatR;

namespace Logistics.Application.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQuery : IRequest<ApiResponse<List<CategoryDto>>>
{
}
