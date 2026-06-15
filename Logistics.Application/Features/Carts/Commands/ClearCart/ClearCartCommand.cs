using Logistics.Application.Common;
using Logistics.Application.DTOs.Carts;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Carts.Commands.ClearCart;

public class ClearCartCommand : IRequest<ApiResponse<CartDto>>
{
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public ClearCartCommand(int currentUserId, UserRole currentUserRole)
    {
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
