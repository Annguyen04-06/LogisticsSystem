using Logistics.Application.Common;
using Logistics.Application.DTOs.Carts;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Carts.Queries.GetMyCart;

public class GetMyCartQuery : IRequest<ApiResponse<CartDto>>
{
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public GetMyCartQuery(int currentUserId, UserRole currentUserRole)
    {
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
