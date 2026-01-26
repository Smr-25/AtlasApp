namespace Atlas.Application.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendVerificationEmailAsync(string to, string code);
    Task SendPasswordResetEmailAsync(string to, string code);
}