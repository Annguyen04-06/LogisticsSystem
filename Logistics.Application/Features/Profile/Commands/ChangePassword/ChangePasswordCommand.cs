using Logistics.Application.Common;
using Logistics.Application.DTOs.Profile;
using MediatR;

namespace Logistics.Application.Features.Profile.Commands.ChangePassword;

public class ChangePasswordCommand(int currentUserId, ChangePasswordDto passwordDto) : IRequest<ApiResponse<bool>>
{
    public int CurrentUserId { get; } = currentUserId;
    public ChangePasswordDto PasswordDto { get; } = passwordDto;
}
