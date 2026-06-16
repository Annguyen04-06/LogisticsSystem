using System.Text.RegularExpressions;
using Logistics.Application.Common;
using Logistics.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Profile.Commands.ChangePassword;

public class ChangePasswordCommandHandler(
    IApplicationDbContext context,
    IPasswordService passwordService) : IRequestHandler<ChangePasswordCommand, ApiResponse<bool>>
{
    private const string StrongPasswordMessage = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.";
    private static readonly Regex StrongPasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$", RegexOptions.Compiled);

    public async Task<ApiResponse<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var dto = request.PasswordDto;
        var validationError = Validate(dto);

        if (validationError is not null)
        {
            return ApiResponse<bool>.Fail(validationError);
        }

        var user = await context.Users
            .FirstOrDefaultAsync(user => user.Id == request.CurrentUserId, cancellationToken);

        if (user is null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy người dùng.");
        }

        if (!passwordService.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
        {
            return ApiResponse<bool>.Fail("Mật khẩu hiện tại không đúng.");
        }

        user.PasswordHash = passwordService.HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true, "Đổi mật khẩu thành công.");
    }

    private static string? Validate(Logistics.Application.DTOs.Profile.ChangePasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
        {
            return "Vui lòng nhập mật khẩu hiện tại.";
        }

        if (string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            return "Vui lòng nhập mật khẩu mới.";
        }

        if (string.IsNullOrWhiteSpace(dto.ConfirmPassword))
        {
            return "Vui lòng xác nhận mật khẩu mới.";
        }

        if (dto.NewPassword != dto.ConfirmPassword)
        {
            return "Mật khẩu xác nhận không khớp.";
        }

        if (!StrongPasswordRegex.IsMatch(dto.NewPassword))
        {
            return StrongPasswordMessage;
        }

        return null;
    }
}
