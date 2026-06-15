using Logistics.Application.Common;
using Logistics.Application.DTOs.Coupons;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Coupons.Commands.ApplyCoupon;

public class ApplyCouponCommand : IRequest<ApiResponse<ApplyCouponResultDto>>
{
    public ApplyCouponDto Coupon { get; }
    public UserRole CurrentUserRole { get; }

    public ApplyCouponCommand(ApplyCouponDto coupon, UserRole currentUserRole)
    {
        Coupon = coupon;
        CurrentUserRole = currentUserRole;
    }
}
