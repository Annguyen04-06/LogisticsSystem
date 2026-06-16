using Logistics.Application.Common;
using Logistics.Application.DTOs.Auth;
using MediatR;

namespace Logistics.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommand(ResetPasswordDto resetPasswordDto) : IRequest<ApiResponse<string>>
{
    public ResetPasswordDto ResetPasswordDto { get; } = resetPasswordDto;
}
