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
        var query = context.Categories.AsQueryable();

        if (!request.IncludeInactive)
        {
            query = query.Where(category => category.IsActive);
        }

        var categories = await query
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
