using Logistics.Application.Common;
using Logistics.Application.DTOs.Profile;
using MediatR;

namespace Logistics.Application.Features.Profile.Queries.GetMyProfile;

public class GetMyProfileQuery(int currentUserId) : IRequest<ApiResponse<ProfileDto>>
{
    public int CurrentUserId { get; } = currentUserId;
}
