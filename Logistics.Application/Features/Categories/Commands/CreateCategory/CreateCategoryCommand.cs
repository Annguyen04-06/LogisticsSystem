using Logistics.Application.Common;
using Logistics.Application.DTOs.Categories;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<ApiResponse<CategoryDto>>
{
    public CreateCategoryDto Category { get; }
    public UserRole CurrentUserRole { get; }

    public CreateCategoryCommand(CreateCategoryDto category, UserRole currentUserRole)
    {
        Category = category;
        CurrentUserRole = currentUserRole;
    }
}
