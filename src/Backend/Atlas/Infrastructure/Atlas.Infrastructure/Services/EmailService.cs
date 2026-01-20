using System.Net;
using System.Net.Mail;
using Atlas.Application.Models;
using Atlas.Application.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Atlas.Infrastructure.Services;

public class EmailService(IOptions<EmailSettings> options) : IEmailService
{
    private readonly EmailSettings _settings = options.Value;

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient(_settings.SmtpServer, _settings.Port);
        client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
        client.EnableSsl = _settings.UseSsl;
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(to);
        await client.SendMailAsync(mailMessage);
    }

    public async Task SendVerificationEmailAsync(string to, string code)
    {
        await SendEmailAsync(to, "Email Verification", code);
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetLink)
    {
        await SendEmailAsync(to, "Password Reset", resetLink);
    }
}