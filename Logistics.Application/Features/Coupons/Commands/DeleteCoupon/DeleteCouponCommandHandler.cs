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
            return ApiResponse<bool>.Fail("Coupon does not exist.");
        }

        coupon.IsActive = false;
        coupon.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Coupon deleted successfully.");
    }
}
