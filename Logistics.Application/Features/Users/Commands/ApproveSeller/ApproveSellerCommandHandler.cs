using Logistics.Application.Common;
using Logistics.Application.DTOs.Users;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Users.Commands.ApproveSeller;

public class ApproveSellerCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ApproveSellerCommand, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(ApproveSellerCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(user => user.Id == request.Id, cancellationToken);

        if (user is null)
        {
            return ApiResponse<UserDto>.Fail("Không tìm thấy người dùng.");
        }

        if (user.Role != UserRole.Seller)
        {
            return ApiResponse<UserDto>.Fail("Chỉ có thể duyệt tài khoản người bán.");
        }

        user.IsApproved = true;
        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<UserDto>.Ok(UserMapping.ToDto(user), "Duyệt người bán thành công.");
    }
}
