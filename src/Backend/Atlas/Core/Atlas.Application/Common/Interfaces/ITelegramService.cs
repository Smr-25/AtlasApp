namespace Atlas.Application.Common.Interfaces;

public interface ITelegramService
{
    Task<bool> SendVerificationCodeAsync(string chatId, string code);
    Task<bool> SendMessageAsync(string chatId, string message);
}