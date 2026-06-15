using Logistics.Application.Common;
using Logistics.Application.DTOs.Coupons;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Coupons.Commands.UpdateCoupon;

public class UpdateCouponCommand : IRequest<ApiResponse<CouponDto>>
{
    public int Id { get; }
    public UpdateCouponDto Coupon { get; }
    public UserRole CurrentUserRole { get; }

    public UpdateCouponCommand(int id, UpdateCouponDto coupon, UserRole currentUserRole)
    {
        Id = id;
        Coupon = coupon;
        CurrentUserRole = currentUserRole;
    }
}
