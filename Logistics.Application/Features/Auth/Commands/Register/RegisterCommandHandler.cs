using Logistics.Application.Common;
using Logistics.Application.DTOs.Auth;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Logistics.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(
    IApplicationDbContext context,
    IPasswordService passwordService,
    IJwtService jwtService) : IRequestHandler<RegisterCommand, ApiResponse<AuthResponseDto>>
{
    private const string StrongPasswordMessage = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.";
    private static readonly Regex StrongPasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$", RegexOptions.Compiled);

    public async Task<ApiResponse<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var dto = request.RegisterDto;
        var validationError = Validate(dto);

        if (validationError is not null)
        {
            return ApiResponse<AuthResponseDto>.Fail(validationError);
        }

        var email = dto.Email.Trim().ToLower();
        var emailExists = await context.Users
            .AnyAsync(user => user.Email.ToLower() == email, cancellationToken);

        if (emailExists)
        {
            return ApiResponse<AuthResponseDto>.Fail("Email đã tồn tại trong hệ thống.");
        }

        var user = new User
        {
            FullName = dto.FullName.Trim(),
            Email = email,
            PasswordHash = passwordService.HashPassword(dto.Password),
            PhoneNumber = dto.PhoneNumber.Trim(),
            Address = dto.Address.Trim(),
            Role = dto.Role,
            IsApproved = dto.Role != UserRole.Seller,
            IsActive = true
        };

        await using var transaction = await context.BeginTransactionAsync(cancellationToken);

        try
        {
            context.Users.Add(user);
            await context.SaveChangesAsync(cancellationToken);

            if (user.Role == UserRole.Customer)
            {
                context.Wallets.Add(new Wallet
                {
                    UserId = user.Id,
                    Balance = 0
                });

                await context.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return ApiResponse<AuthResponseDto>.Fail("Có lỗi xảy ra khi đăng ký tài khoản. Vui lòng thử lại.");
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return ApiResponse<AuthResponseDto>.Fail("Có lỗi xảy ra khi đăng ký tài khoản. Vui lòng thử lại.");
        }

        var response = CreateAuthResponse(user, jwtService.GenerateToken(user));
        return ApiResponse<AuthResponseDto>.Ok(response, "Registration successful.");
    }

    private static string? Validate(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FullName))
        {
            return "Vui lòng nhập họ và tên.";
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return "Vui lòng nhập email.";
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return "Vui lòng nhập mật khẩu.";
        }

        if (!StrongPasswordRegex.IsMatch(dto.Password))
        {
            return StrongPasswordMessage;
        }

        if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            return "Vui lòng nhập số điện thoại.";
        }

        if (string.IsNullOrWhiteSpace(dto.Address))
        {
            return "Vui lòng nhập địa chỉ.";
        }

        if (dto.Role is not UserRole.Customer and not UserRole.Seller)
        {
            return "Vai trò không hợp lệ.";
        }

        return null;
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
