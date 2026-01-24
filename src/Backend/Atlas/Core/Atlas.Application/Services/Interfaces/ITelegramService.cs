namespace Atlas.Application.Services.Interfaces;

public interface ITelegramService
{
    Task<bool> SendVerificationCodeAsync(string chatId, string code);
}