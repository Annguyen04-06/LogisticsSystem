using Logistics.Application.Common;
using Logistics.Application.DTOs.Auth;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(
    IApplicationDbContext context,
    IPasswordService passwordService,
    IJwtService jwtService) : IRequestHandler<LoginCommand, ApiResponse<AuthResponseDto>>
{
    public async Task<ApiResponse<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var dto = request.LoginDto;

        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return ApiResponse<AuthResponseDto>.Fail("Email and password are required.");
        }

        var email = dto.Email.Trim().ToLower();
        var user = await context.Users
            .FirstOrDefaultAsync(user => user.Email.ToLower() == email, cancellationToken);

        if (user is null)
        {
            return ApiResponse<AuthResponseDto>.Fail("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            return ApiResponse<AuthResponseDto>.Fail("Tài khoản đã bị vô hiệu hóa.");
        }

        if (!passwordService.VerifyPassword(dto.Password, user.PasswordHash))
        {
            return ApiResponse<AuthResponseDto>.Fail("Invalid email or password.");
        }

        if (user.Role == UserRole.Seller && !user.IsApproved)
        {
            return ApiResponse<AuthResponseDto>.Fail("Tài khoản người bán đang chờ quản trị viên phê duyệt.");
        }

        var response = CreateAuthResponse(user, jwtService.GenerateToken(user));
        return ApiResponse<AuthResponseDto>.Ok(response, "Login successful.");
    }

    private static AuthResponseDto CreateAuthResponse(User user, string token)
    {
        return new AuthResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = token
        };
    }
}
