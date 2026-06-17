using Logistics.Application.Common;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Coupons.Commands.DeleteCoupon;

public class DeleteCouponCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteCouponCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<bool>.Fail("Only admin can delete coupons.");
        }

        var coupon = await context.Coupons
            .FirstOrDefaultAsync(coupon => coupon.Id == request.Id, cancellationToken);

        if (coupon is null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy mã giảm giá.");
        }

        var hasUsageHistory = await context.CouponUsages
            .AnyAsync(couponUsage => couponUsage.CouponId == coupon.Id, cancellationToken);

        if (hasUsageHistory)
        {
            coupon.IsActive = false;
            coupon.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(
                true,
                "Mã giảm giá đã có lịch sử sử dụng nên được chuyển sang trạng thái ngừng hoạt động để giữ lịch sử người mua.");
        }

        context.Coupons.Remove(coupon);

        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Đã xóa mã giảm giá khỏi hệ thống.");
    }
}
