using Logistics.Application.Common;
using Logistics.Application.DTOs.Auth;
using MediatR;

namespace Logistics.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommand(ForgotPasswordDto forgotPasswordDto) : IRequest<ApiResponse<ForgotPasswordResponseDto>>
{
    public ForgotPasswordDto ForgotPasswordDto { get; } = forgotPasswordDto;
}
