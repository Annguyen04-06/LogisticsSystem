using Logistics.Application.Common;
using Logistics.Application.DTOs.Profile;
using Logistics.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Profile.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateMyProfileCommand, ApiResponse<ProfileDto>>
{
    public async Task<ApiResponse<ProfileDto>> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        var dto = request.ProfileDto;

        if (string.IsNullOrWhiteSpace(dto.FullName))
        {
            return ApiResponse<ProfileDto>.Fail("Vui lòng nhập họ và tên.");
        }

        if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            return ApiResponse<ProfileDto>.Fail("Vui lòng nhập số điện thoại.");
        }

        if (string.IsNullOrWhiteSpace(dto.Address))
        {
            return ApiResponse<ProfileDto>.Fail("Vui lòng nhập địa chỉ.");
        }

        var user = await context.Users
            .FirstOrDefaultAsync(user => user.Id == request.CurrentUserId, cancellationToken);

        if (user is null)
        {
            return ApiResponse<ProfileDto>.Fail("Không tìm thấy người dùng.");
        }

        user.FullName = dto.FullName.Trim();
        user.PhoneNumber = dto.PhoneNumber.Trim();
        user.Address = dto.Address.Trim();
        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<ProfileDto>.Ok(ProfileMapping.ToDto(user), "Cập nhật thông tin cá nhân thành công.");
    }
}
