using Logistics.Application.Common;
using Logistics.Application.DTOs.Auth;
using MediatR;

namespace Logistics.Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<ApiResponse<AuthResponseDto>>
{
    public LoginDto LoginDto { get; }

    public LoginCommand(LoginDto loginDto)
    {
        LoginDto = loginDto;
    }
}
