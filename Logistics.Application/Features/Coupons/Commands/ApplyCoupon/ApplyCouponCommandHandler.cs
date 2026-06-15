using Logistics.Application.Common;
using Logistics.Application.DTOs.Coupons;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Coupons.Commands.ApplyCoupon;

public class ApplyCouponCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ApplyCouponCommand, ApiResponse<ApplyCouponResultDto>>
{
    public async Task<ApiResponse<ApplyCouponResultDto>> Handle(ApplyCouponCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<ApplyCouponResultDto>.Fail("Only customer can apply coupons.");
        }

        if (string.IsNullOrWhiteSpace(request.Coupon.Code))
        {
            return ApiResponse<ApplyCouponResultDto>.Fail("Coupon code is required.");
        }

        if (request.Coupon.OrderAmount <= 0)
        {
            return ApiResponse<ApplyCouponResultDto>.Fail("Order amount must be greater than 0.");
        }

        var code = request.Coupon.Code.Trim().ToUpper();
        var coupon = await context.Coupons
            .FirstOrDefaultAsync(coupon => coupon.Code.ToUpper() == code, cancellationToken);

        if (coupon is null)
        {
            return ApiResponse<ApplyCouponResultDto>.Fail("Coupon does not exist.");
        }

        var validationError = CouponHelper.ValidateCanApply(coupon, request.Coupon.OrderAmount, DateTime.UtcNow);
        if (validationError is not null)
        {
            return ApiResponse<ApplyCouponResultDto>.Fail(validationError);
        }

        var discountAmount = CouponHelper.CalculateDiscount(coupon, request.Coupon.OrderAmount);

        return ApiResponse<ApplyCouponResultDto>.Ok(new ApplyCouponResultDto
        {
            Code = coupon.Code,
            OrderAmount = request.Coupon.OrderAmount,
            DiscountAmount = discountAmount,
            FinalAmount = request.Coupon.OrderAmount - discountAmount
        });
    }
}
