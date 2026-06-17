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
            return ApiResponse<bool>.Fail("Không tìm thấy sản phẩm.");
        }

        if (request.CurrentUserRole != UserRole.Admin && product.SellerId != request.CurrentUserId)
        {
            return ApiResponse<bool>.Fail("Không có quyền xóa sản phẩm này.");
        }

        var hasOrderDetails = await context.OrderDetails
            .AnyAsync(detail => detail.ProductId == product.Id, cancellationToken);

        if (!hasOrderDetails)
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Đã xóa sản phẩm khỏi hệ thống.");
        }

        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Sản phẩm đã phát sinh đơn hàng nên được chuyển sang trạng thái ngừng bán để giữ lịch sử mua hàng.");
    }
}
