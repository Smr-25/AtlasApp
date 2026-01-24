using Atlas.Application.Services.Interfaces;
using Atlas.Application.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Atlas.Infrastructure.Services;

public class TelegramService : ITelegramService
{
    private readonly TelegramBotClient _botClient;
    private readonly TelegramSettings _settings;

    public TelegramService(IOptions<TelegramSettings> telegramSettings)
    {
        _settings = telegramSettings.Value;
        _botClient = new TelegramBotClient(_settings.BotToken);
    }


    public async Task<bool> SendVerificationCodeAsync(string chatId, string code)
    {
        var message = $"Your verification code is: {code}";
        await _botClient.SendMessage(chatId, message);
        return true;
    }

    public async Task<bool> SendPasswordResetCodeAsync(string chatId, string code)
    {
        var message = $"Your password reset code is: {code}";
        await _botClient.SendMessage(chatId, message);
        return true;
    }

    public Task<string> GetBotLinkAsync(string payload)
    {
        var botLink = $"https://t.me/{_settings.BotUsername}?start={payload}";
        return Task.FromResult(botLink);
    }
}