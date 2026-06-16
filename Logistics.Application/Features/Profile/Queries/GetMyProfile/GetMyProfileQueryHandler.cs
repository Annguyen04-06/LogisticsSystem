using Logistics.Application.Common;
using Logistics.Application.DTOs.Profile;
using Logistics.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Profile.Queries.GetMyProfile;

public class GetMyProfileQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMyProfileQuery, ApiResponse<ProfileDto>>
{
    public async Task<ApiResponse<ProfileDto>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(user => user.Id == request.CurrentUserId, cancellationToken);

        if (user is null)
        {
            return ApiResponse<ProfileDto>.Fail("Không tìm thấy người dùng.");
        }

        return ApiResponse<ProfileDto>.Ok(ProfileMapping.ToDto(user));
    }
}
