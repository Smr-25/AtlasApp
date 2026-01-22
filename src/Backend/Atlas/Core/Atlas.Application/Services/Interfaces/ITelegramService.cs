namespace Atlas.Application.Services.Interfaces;

public interface ITelegramService
{
    Task<bool> SendVerificationCodeAsync(string chatId, string code);
    Task<bool> SendPasswordResetCodeAsync(string chatId, string code);
    Task<string> GetBotLinkAsync(string payload);
}