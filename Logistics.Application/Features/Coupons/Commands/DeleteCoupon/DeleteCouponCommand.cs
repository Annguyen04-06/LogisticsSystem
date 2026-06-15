using Logistics.Application.Common;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Coupons.Commands.DeleteCoupon;

public class DeleteCouponCommand : IRequest<ApiResponse<bool>>
{
    public int Id { get; }
    public UserRole CurrentUserRole { get; }

    public DeleteCouponCommand(int id, UserRole currentUserRole)
    {
        Id = id;
        CurrentUserRole = currentUserRole;
    }
}
