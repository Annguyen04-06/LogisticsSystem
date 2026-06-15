using Logistics.Application.Common;
using Logistics.Application.DTOs.Categories;
using Logistics.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCategoryByIdQuery, ApiResponse<CategoryDto>>
{
    public async Task<ApiResponse<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .Where(category => category.Id == request.Id && category.IsActive)
            .Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            return ApiResponse<CategoryDto>.Fail("Category does not exist.");
        }

        return ApiResponse<CategoryDto>.Ok(category);
    }
}
