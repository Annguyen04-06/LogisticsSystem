using System.Text.RegularExpressions;
using Logistics.Application.Common;
using Logistics.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler(
    IApplicationDbContext context,
    IPasswordService passwordService) : IRequestHandler<ResetPasswordCommand, ApiResponse<string>>
{
    private const string StrongPasswordMessage = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.";
    private static readonly Regex StrongPasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$", RegexOptions.Compiled);

    public async Task<ApiResponse<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var dto = request.ResetPasswordDto;
        var validationError = Validate(dto);

        if (validationError is not null)
        {
            return ApiResponse<string>.Fail(validationError);
        }

        var email = dto.Email.Trim().ToLower();
        var tokenValue = dto.Token.Trim();

        var user = await context.Users
            .FirstOrDefaultAsync(user => user.Email.ToLower() == email, cancellationToken);

        if (user is null)
        {
            return ApiResponse<string>.Fail("Mã đặt lại mật khẩu không hợp lệ.");
        }

        var resetToken = await context.PasswordResetTokens
            .Where(token => token.UserId == user.Id && token.Token == tokenValue)
            .OrderByDescending(token => token.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (resetToken is null)
        {
            return ApiResponse<string>.Fail("Mã đặt lại mật khẩu không hợp lệ.");
        }

        if (resetToken.IsUsed)
        {
            return ApiResponse<string>.Fail("Mã đặt lại mật khẩu đã được sử dụng.");
        }

        if (resetToken.ExpiredAt < DateTime.Now)
        {
            return ApiResponse<string>.Fail("Mã đặt lại mật khẩu đã hết hạn.");
        }

        user.PasswordHash = passwordService.HashPassword(dto.NewPassword);
        resetToken.IsUsed = true;
        resetToken.UsedAt = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        return ApiResponse<string>.Ok(string.Empty, "Đặt lại mật khẩu thành công. Vui lòng đăng nhập.");
    }

    private static string? Validate(Logistics.Application.DTOs.Auth.ResetPasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return "Vui lòng nhập email.";
        }

        if (string.IsNullOrWhiteSpace(dto.Token))
        {
            return "Vui lòng nhập mã đặt lại mật khẩu.";
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
