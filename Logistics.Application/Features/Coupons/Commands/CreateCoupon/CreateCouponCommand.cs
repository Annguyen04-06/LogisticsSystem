using Logistics.Application.Common;
using Logistics.Application.DTOs.Coupons;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Coupons.Commands.CreateCoupon;

public class CreateCouponCommand : IRequest<ApiResponse<CouponDto>>
{
    public CreateCouponDto Coupon { get; }
    public UserRole CurrentUserRole { get; }

    public CreateCouponCommand(CreateCouponDto coupon, UserRole currentUserRole)
    {
        Coupon = coupon;
        CurrentUserRole = currentUserRole;
    }
}
