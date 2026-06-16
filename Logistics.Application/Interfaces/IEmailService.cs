namespace Logistics.Application.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(
        string toEmail,
        string fullName,
        string token,
        DateTime expiredAt,
        CancellationToken cancellationToken);
}
