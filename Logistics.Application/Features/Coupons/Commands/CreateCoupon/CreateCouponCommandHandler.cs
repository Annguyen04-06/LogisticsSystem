using Logistics.Application.Common;
using Logistics.Application.DTOs.Coupons;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Coupons.Commands.CreateCoupon;

public class CreateCouponCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateCouponCommand, ApiResponse<CouponDto>>
{
    public async Task<ApiResponse<CouponDto>> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<CouponDto>.Fail("Only admin can create coupons.");
        }

        if (string.IsNullOrWhiteSpace(request.Coupon.Code))
        {
            return ApiResponse<CouponDto>.Fail("Coupon code is required.");
        }

        var code = request.Coupon.Code.Trim().ToUpper();
        var exists = await context.Coupons
            .AnyAsync(coupon => coupon.Code.ToUpper() == code, cancellationToken);

        if (exists)
        {
            return ApiResponse<CouponDto>.Fail("Coupon code already exists.");
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

        var coupon = new Coupon
        {
            Code = code,
            DiscountType = request.Coupon.DiscountType,
            DiscountValue = request.Coupon.DiscountValue,
            MinOrderAmount = request.Coupon.MinOrderAmount,
            StartDate = startDateUtc,
            EndDate = endDateUtc,
            UsageLimit = request.Coupon.UsageLimit,
            UsedCount = 0,
            IsActive = request.Coupon.IsActive
        };

        context.Coupons.Add(coupon);
        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<CouponDto>.Ok(CouponHelper.ToDto(coupon), "Coupon created successfully.");
    }
}
