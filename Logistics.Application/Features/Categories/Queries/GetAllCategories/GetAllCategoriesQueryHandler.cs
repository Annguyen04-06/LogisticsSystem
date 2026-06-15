using Logistics.Application.Common;
using Logistics.Application.DTOs.Categories;
using Logistics.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllCategoriesQuery, ApiResponse<List<CategoryDto>>>
{
    public async Task<ApiResponse<List<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await context.Categories
            .Where(category => category.IsActive)
            .OrderBy(category => category.Name)
            .Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<CategoryDto>>.Ok(categories);
    }
}
