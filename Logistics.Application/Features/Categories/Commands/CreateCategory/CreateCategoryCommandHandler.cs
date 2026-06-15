using Logistics.Application.Common;
using Logistics.Application.DTOs.Categories;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateCategoryCommand, ApiResponse<CategoryDto>>
{
    public async Task<ApiResponse<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<CategoryDto>.Fail("Only admin can create categories.");
        }

        if (string.IsNullOrWhiteSpace(request.Category.Name))
        {
            return ApiResponse<CategoryDto>.Fail("Category name is required.");
        }

        var category = new Category
        {
            Name = request.Category.Name.Trim(),
            Description = request.Category.Description.Trim(),
            IsActive = true
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<CategoryDto>.Ok(new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive
        }, "Category created successfully.");
    }
}
