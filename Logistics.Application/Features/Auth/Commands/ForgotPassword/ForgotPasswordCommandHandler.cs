using System.Security.Cryptography;
using Logistics.Application.Common;
using Logistics.Application.DTOs.Auth;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler(
    IApplicationDbContext context,
    IEmailService emailService) : IRequestHandler<ForgotPasswordCommand, ApiResponse<ForgotPasswordResponseDto>>
{
    private const string SuccessMessage = "Nếu email tồn tại trong hệ thống, hướng dẫn đặt lại mật khẩu đã được gửi.";
    private const string SendEmailErrorMessage = "Không thể gửi email đặt lại mật khẩu. Vui lòng thử lại sau.";

    public async Task<ApiResponse<ForgotPasswordResponseDto>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var dto = request.ForgotPasswordDto;

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return ApiResponse<ForgotPasswordResponseDto>.Fail("Vui lòng nhập email.");
        }

        var email = dto.Email.Trim().ToLower();
        var expiredAt = DateTime.Now.AddMinutes(15);
        var response = new ForgotPasswordResponseDto
        {
            Email = email,
            ExpiredAt = expiredAt
        };

        var user = await context.Users
            .FirstOrDefaultAsync(user => user.Email.ToLower() == email, cancellationToken);

        if (user is null)
        {
            return ApiResponse<ForgotPasswordResponseDto>.Ok(response, SuccessMessage);
        }

        var token = GenerateResetToken();

        context.PasswordResetTokens.Add(new PasswordResetToken
        {
            UserId = user.Id,
            Token = token,
            ExpiredAt = expiredAt,
            IsUsed = false,
            CreatedAt = DateTime.Now
        });

        await context.SaveChangesAsync(cancellationToken);

        try
        {
            await emailService.SendPasswordResetEmailAsync(
                user.Email,
                user.FullName,
                token,
                expiredAt,
                cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return ApiResponse<ForgotPasswordResponseDto>.Fail(SendEmailErrorMessage);
        }

        return ApiResponse<ForgotPasswordResponseDto>.Ok(response, SuccessMessage);
    }

    private static string GenerateResetToken()
    {
        return RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
    }
}
