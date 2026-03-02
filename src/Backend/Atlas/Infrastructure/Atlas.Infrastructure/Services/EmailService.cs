using Atlas.Application.Common.Interfaces;
using Atlas.Application.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Atlas.Infrastructure.Services;

public class EmailService(IOptions<EmailSettings> options) : IEmailService
{
    private readonly EmailSettings _settings = options.Value;

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    public async Task SendVerificationEmailAsync(string to, string code)
    {
        var html = BuildEmailTemplate(
            "Verify Your Email",
            "Welcome to ATLAS!",
            $"""
             <p style="font-size:16px;color:#333;">Thank you for signing up. Please use the verification code below to confirm your email address:</p>
             <div style="text-align:center;margin:32px 0;">
                 <span style="display:inline-block;background:linear-gradient(135deg,#f97316,#ea580c);color:#fff;font-size:32px;font-weight:700;letter-spacing:8px;padding:16px 32px;border-radius:12px;box-shadow:0 4px 14px rgba(249,115,22,0.35);">{code}</span>
             </div>
             <p style="font-size:14px;color:#666;">This code will expire in <strong>15 minutes</strong>. If you didn't create an account, you can safely ignore this email.</p>
             """);
        await SendEmailAsync(to, "ATLAS — Verify Your Email Address", html);
    }

    public async Task SendPasswordResetEmailAsync(string to, string code)
    {
        var html = BuildEmailTemplate(
            "Password Reset",
            "Reset Your Password",
            $"""
            <p style="font-size:16px;color:#333;">We received a request to reset your password. Use the code below to proceed:</p>
            <div style="text-align:center;margin:32px 0;">
                <span style="display:inline-block;background:#ef4444;color:#fff;font-size:32px;font-weight:700;letter-spacing:8px;padding:16px 32px;border-radius:12px;">{code}</span>
            </div>
            <p style="font-size:14px;color:#666;">This code will expire in <strong>10 minutes</strong>. If you didn't request a password reset, please secure your account immediately.</p>
            """);
        await SendEmailAsync(to, "ATLAS — Password Reset Code", html);
    }

    private static string BuildEmailTemplate(string preheader, string heading, string bodyContent)
    {
        return $"""
        <!DOCTYPE html>
        <html lang="en">
        <head><meta charset="UTF-8"><meta name="viewport" content="width=device-width,initial-scale=1.0">
        <title>{preheader}</title></head>
        <body style="margin:0;padding:0;background:#f4f4f5;font-family:'Segoe UI',Roboto,Helvetica,Arial,sans-serif;">
          <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background:#f4f4f5;padding:40px 0;">
            <tr><td align="center">
              <table role="presentation" width="560" cellpadding="0" cellspacing="0" style="background:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,0.08);">
                <tr>
                  <td style="background:linear-gradient(135deg,#6366f1,#8b5cf6);padding:32px 40px;text-align:center;">
                    <h1 style="margin:0;color:#ffffff;font-size:28px;font-weight:700;letter-spacing:-0.5px;">⚡ ATLAS</h1>
                    <p style="margin:4px 0 0;color:rgba(255,255,255,0.85);font-size:13px;">Developer Workspace Platform</p>
                  </td>
                </tr>
                <tr>
                  <td style="padding:40px;">
                    <h2 style="margin:0 0 16px;color:#18181b;font-size:22px;font-weight:600;">{heading}</h2>
                    {bodyContent}
                  </td>
                </tr>
                <tr>
                  <td style="padding:24px 40px;background:#fafafa;border-top:1px solid #e4e4e7;text-align:center;">
                    <p style="margin:0;font-size:12px;color:#a1a1aa;">© 2026 ATLAS. All rights reserved.</p>
                    <p style="margin:4px 0 0;font-size:12px;color:#a1a1aa;">This is an automated message — please do not reply directly.</p>
                  </td>
                </tr>
              </table>
            </td></tr>
          </table>
        </body>
        </html>
        """;
    }
}