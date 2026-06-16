using Logistics.Application.Common;
using Logistics.Application.DTOs.Profile;
using MediatR;

namespace Logistics.Application.Features.Profile.Commands.UpdateMyProfile;

public class UpdateMyProfileCommand(int currentUserId, UpdateProfileDto profileDto) : IRequest<ApiResponse<ProfileDto>>
{
    public int CurrentUserId { get; } = currentUserId;
    public UpdateProfileDto ProfileDto { get; } = profileDto;
}
