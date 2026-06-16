using Logistics.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Logistics.Infrastructure.Services;

public class EmailService(IConfiguration configuration) : IEmailService
{
    public async Task SendPasswordResetEmailAsync(
        string toEmail,
        string fullName,
        string token,
        DateTime expiredAt,
        CancellationToken cancellationToken)
    {
        var host = configuration["Smtp:Host"];
        var userName = configuration["Smtp:UserName"];
        var password = configuration["Smtp:Password"];
        var fromName = configuration["Smtp:FromName"] ?? "Hệ thống quản lý Logistics";
        var port = configuration.GetValue<int>("Smtp:Port");
        var enableSsl = configuration.GetValue<bool>("Smtp:EnableSsl");

        if (string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(password) ||
            port <= 0)
        {
            throw new InvalidOperationException("SMTP configuration is missing.");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, userName));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Đặt lại mật khẩu - Hệ thống quản lý Logistics";

        message.Body = new TextPart("html")
        {
            Text = $"""
                   <p>Xin chào {fullName},</p>
                   <p>Bạn vừa yêu cầu đặt lại mật khẩu cho tài khoản trong Hệ thống quản lý Logistics.</p>
                   <p>Mã đặt lại mật khẩu của bạn là:</p>
                   <h2 style="letter-spacing: 4px;">{token}</h2>
                   <p>Mã này sẽ hết hạn lúc <strong>{expiredAt:dd/MM/yyyy HH:mm}</strong>, tức trong 15 phút.</p>
                   <p>Vui lòng không chia sẻ mã này cho bất kỳ ai.</p>
                   """
        };

        using var client = new SmtpClient();
        var secureSocketOptions = enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

        await client.ConnectAsync(host, port, secureSocketOptions, cancellationToken);
        await client.AuthenticateAsync(userName, password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
