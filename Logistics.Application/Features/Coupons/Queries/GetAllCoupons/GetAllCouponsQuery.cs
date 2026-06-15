using Logistics.Application.Common;
using Logistics.Application.DTOs.Coupons;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Coupons.Queries.GetAllCoupons;

public class GetAllCouponsQuery : IRequest<ApiResponse<List<CouponDto>>>
{
    public UserRole CurrentUserRole { get; }

    public GetAllCouponsQuery(UserRole currentUserRole)
    {
        CurrentUserRole = currentUserRole;
    }
}
