using Logistics.Application.Common;
using Logistics.Application.DTOs.Users;
using Logistics.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Users.Commands.ToggleUserActive;

public class ToggleUserActiveCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ToggleUserActiveCommand, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(ToggleUserActiveCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(user => user.Id == request.Id, cancellationToken);

        if (user is null)
        {
            return ApiResponse<UserDto>.Fail("Không tìm thấy người dùng.");
        }

        if (user.Id == request.CurrentUserId && !request.IsActive)
        {
            return ApiResponse<UserDto>.Fail("Quản trị viên không thể vô hiệu hóa chính mình.");
        }

        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        var message = request.IsActive
            ? "Kích hoạt tài khoản thành công."
            : "Vô hiệu hóa tài khoản thành công.";

        return ApiResponse<UserDto>.Ok(UserMapping.ToDto(user), message);
    }
}
