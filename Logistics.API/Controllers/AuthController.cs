using Logistics.Application.DTOs.Auth;
using Logistics.Application.Features.Auth.Commands.ForgotPassword;
using Logistics.Application.Features.Auth.Commands.Login;
using Logistics.Application.Features.Auth.Commands.Register;
using Logistics.Application.Features.Auth.Commands.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new RegisterCommand(registerDto), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new LoginCommand(loginDto), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ForgotPasswordCommand(forgotPasswordDto), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ResetPasswordCommand(resetPasswordDto), cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
