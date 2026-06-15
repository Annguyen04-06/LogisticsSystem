using Logistics.Application.Common;
using Logistics.Application.DTOs.Auth;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(
    IApplicationDbContext context,
    IPasswordService passwordService,
    IJwtService jwtService) : IRequestHandler<RegisterCommand, ApiResponse<AuthResponseDto>>
{
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
            return ApiResponse<AuthResponseDto>.Fail("Email is already registered.");
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

        context.Users.Add(user);

        if (user.Role == UserRole.Customer)
        {
            context.Wallets.Add(new Wallet
            {
                UserId = user.Id,
                Balance = 0
            });
        }

        await context.SaveChangesAsync(cancellationToken);

        var response = CreateAuthResponse(user, jwtService.GenerateToken(user));
        return ApiResponse<AuthResponseDto>.Ok(response, "Registration successful.");
    }

    private static string? Validate(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FullName))
        {
            return "Full name is required.";
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return "Email is required.";
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return "Password is required.";
        }

        if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            return "Phone number is required.";
        }

        if (string.IsNullOrWhiteSpace(dto.Address))
        {
            return "Address is required.";
        }

        if (!Enum.IsDefined(dto.Role))
        {
            return "Role is invalid.";
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
