using Logistics.Application.Common;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteCategoryCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<bool>.Fail("Chỉ quản trị viên mới có quyền xóa danh mục.");
        }

        var category = await context.Categories
            .FirstOrDefaultAsync(category => category.Id == request.Id, cancellationToken);

        if (category is null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy danh mục.");
        }

        var hasProducts = await context.Products
            .AnyAsync(product => product.CategoryId == category.Id, cancellationToken);

        if (hasProducts)
        {
            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Danh mục đã có sản phẩm nên được chuyển sang trạng thái ngừng sử dụng.");
        }

        context.Categories.Remove(category);

        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Xóa danh mục thành công.");
    }
}
