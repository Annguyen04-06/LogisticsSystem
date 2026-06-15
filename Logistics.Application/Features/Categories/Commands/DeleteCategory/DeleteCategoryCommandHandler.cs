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
            return ApiResponse<bool>.Fail("Only admin can delete categories.");
        }

        var category = await context.Categories
            .FirstOrDefaultAsync(category => category.Id == request.Id, cancellationToken);

        if (category is null)
        {
            return ApiResponse<bool>.Fail("Category does not exist.");
        }

        var hasProducts = await context.Products
            .AnyAsync(product => product.CategoryId == category.Id, cancellationToken);

        if (hasProducts)
        {
            return ApiResponse<bool>.Fail("Cannot delete category because it has products.");
        }

        category.IsActive = false;
        category.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Category deleted successfully.");
    }
}
