namespace Atlas.Application.Services.Interfaces;

public interface ISmsService
{
    Task SendSmsAsync(string to, string code);
    Task SendVerificationSmsAsync(string to, string code);
}