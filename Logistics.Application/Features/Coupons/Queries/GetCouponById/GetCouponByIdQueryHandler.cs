using Logistics.Application.Common;
using Logistics.Application.DTOs.Coupons;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Coupons.Queries.GetCouponById;

public class GetCouponByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCouponByIdQuery, ApiResponse<CouponDto>>
{
    public async Task<ApiResponse<CouponDto>> Handle(GetCouponByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Admin)
        {
            return ApiResponse<CouponDto>.Fail("Only admin can view coupons.");
        }

        var coupon = await context.Coupons
            .FirstOrDefaultAsync(coupon => coupon.Id == request.Id, cancellationToken);

        if (coupon is null)
        {
            return ApiResponse<CouponDto>.Fail("Coupon does not exist.");
        }

        return ApiResponse<CouponDto>.Ok(CouponHelper.ToDto(coupon));
    }
}
