using Logistics.Application.Common;
using Logistics.Application.DTOs.Auth;
using MediatR;

namespace Logistics.Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<ApiResponse<AuthResponseDto>>
{
    public RegisterDto RegisterDto { get; }

    public RegisterCommand(RegisterDto registerDto)
    {
        RegisterDto = registerDto;
    }
}
