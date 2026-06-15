using Logistics.Application.Common;
using Logistics.Application.DTOs.Categories;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<ApiResponse<CategoryDto>>
{
    public int Id { get; }
    public UpdateCategoryDto Category { get; }
    public UserRole CurrentUserRole { get; }

    public UpdateCategoryCommand(int id, UpdateCategoryDto category, UserRole currentUserRole)
    {
        Id = id;
        Category = category;
        CurrentUserRole = currentUserRole;
    }
}
