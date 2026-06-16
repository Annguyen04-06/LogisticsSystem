using Logistics.Application.Common;
using Logistics.Application.DTOs.Users;
using MediatR;

namespace Logistics.Application.Features.Users.Commands.ToggleUserActive;

public class ToggleUserActiveCommand(int id, bool isActive, int currentUserId) : IRequest<ApiResponse<UserDto>>
{
    public int Id { get; } = id;
    public bool IsActive { get; } = isActive;
    public int CurrentUserId { get; } = currentUserId;
}
