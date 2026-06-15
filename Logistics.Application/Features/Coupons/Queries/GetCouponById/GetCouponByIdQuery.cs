using Logistics.Application.Common;
using Logistics.Application.DTOs.Coupons;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Coupons.Queries.GetCouponById;

public class GetCouponByIdQuery : IRequest<ApiResponse<CouponDto>>
{
    public int Id { get; }
    public UserRole CurrentUserRole { get; }

    public GetCouponByIdQuery(int id, UserRole currentUserRole)
    {
        Id = id;
        CurrentUserRole = currentUserRole;
    }
}
