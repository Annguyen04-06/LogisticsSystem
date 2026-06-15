using Logistics.Application.Common;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteProductCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(product => product.Id == request.Id, cancellationToken);

        if (product is null)
        {
            return ApiResponse<bool>.Fail("Product does not exist.");
        }

        if (request.CurrentUserRole != UserRole.Admin && product.SellerId != request.CurrentUserId)
        {
            return ApiResponse<bool>.Fail("You cannot delete another seller's product.");
        }

        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Product deleted successfully.");
    }
}
