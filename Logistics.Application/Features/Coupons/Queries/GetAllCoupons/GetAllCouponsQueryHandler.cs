using Logistics.Application.Common;
using Logistics.Application.DTOs.Coupons;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Coupons.Queries.GetAllCoupons;

public class GetAllCouponsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllCouponsQuery, ApiResponse<List<CouponDto>>>
{
    public async Task<ApiResponse<List<CouponDto>>> Handle(GetAllCouponsQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<List<CouponDto>>.Fail("Only admin can view coupons.");
        }

        var coupons = await context.Coupons
            .OrderByDescending(coupon => coupon.CreatedAt)
            .Select(coupon => new CouponDto
            {
                Id = coupon.Id,
                Code = coupon.Code,
                DiscountType = coupon.DiscountType,
                DiscountValue = coupon.DiscountValue,
                MinOrderAmount = coupon.MinOrderAmount,
                StartDate = coupon.StartDate,
                EndDate = coupon.EndDate,
                UsageLimit = coupon.UsageLimit,
                UsedCount = coupon.UsedCount,
                IsActive = coupon.IsActive
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<CouponDto>>.Ok(coupons);
    }
}
