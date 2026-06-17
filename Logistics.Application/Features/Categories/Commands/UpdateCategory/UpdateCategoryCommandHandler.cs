using Logistics.Application.Common;
using Logistics.Application.DTOs.Categories;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateCategoryCommand, ApiResponse<CategoryDto>>
{
    public async Task<ApiResponse<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<CategoryDto>.Fail("Chỉ quản trị viên mới có quyền cập nhật danh mục.");
        }

        if (string.IsNullOrWhiteSpace(request.Category.Name))
        {
            return ApiResponse<CategoryDto>.Fail("Vui lòng nhập tên danh mục.");
        }

        var category = await context.Categories
            .FirstOrDefaultAsync(category => category.Id == request.Id, cancellationToken);

        if (category is null)
        {
            return ApiResponse<CategoryDto>.Fail("Không tìm thấy danh mục.");
        }

        category.Name = request.Category.Name.Trim();
        category.Description = request.Category.Description.Trim();
        category.IsActive = request.Category.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<CategoryDto>.Ok(new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive
        }, "Cập nhật danh mục thành công.");
    }
}
