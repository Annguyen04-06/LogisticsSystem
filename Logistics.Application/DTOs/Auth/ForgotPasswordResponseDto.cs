namespace Logistics.Application.DTOs.Auth;

public class ForgotPasswordResponseDto
{
    public string Email { get; set; } = string.Empty;
    public DateTime ExpiredAt { get; set; }
}
