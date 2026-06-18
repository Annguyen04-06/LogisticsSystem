using Logistics.Application.Common;
using Logistics.Application.DTOs.Coupons;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Coupons.Commands.UpdateCoupon;

public class UpdateCouponCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateCouponCommand, ApiResponse<CouponDto>>
{
    public async Task<ApiResponse<CouponDto>> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<CouponDto>.Fail("Only admin can update coupons.");
        }

        var coupon = await context.Coupons
            .FirstOrDefaultAsync(coupon => coupon.Id == request.Id, cancellationToken);

        if (coupon is null)
        {
            return ApiResponse<CouponDto>.Fail("Coupon does not exist.");
        }

        var startDateUtc = VietnamTime.ToUtc(request.Coupon.StartDate);
        var endDateUtc = VietnamTime.ToUtc(request.Coupon.EndDate);

        var validationError = CouponHelper.ValidateCouponFields(
            request.Coupon.DiscountType,
            request.Coupon.DiscountValue,
            request.Coupon.MinOrderAmount,
            startDateUtc,
            endDateUtc,
            request.Coupon.UsageLimit);

        if (validationError is not null)
        {
            return ApiResponse<CouponDto>.Fail(validationError);
        }

        coupon.DiscountType = request.Coupon.DiscountType;
        coupon.DiscountValue = request.Coupon.DiscountValue;
        coupon.MinOrderAmount = request.Coupon.MinOrderAmount;
        coupon.StartDate = startDateUtc;
        coupon.EndDate = endDateUtc;
        coupon.UsageLimit = request.Coupon.UsageLimit;
        coupon.IsActive = request.Coupon.IsActive;
        coupon.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<CouponDto>.Ok(CouponHelper.ToDto(coupon), "Coupon updated successfully.");
    }
}
