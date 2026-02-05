using Atlas.Application.Common.Interfaces;
using Atlas.Application.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Atlas.Infrastructure.Services;

public class TelegramService : ITelegramService
{
    private readonly TelegramBotClient _botClient;

    public TelegramService(IOptions<TelegramSettings> telegramSettings)
    {
        var settings = telegramSettings.Value;
        _botClient = new TelegramBotClient(settings.BotToken);
    }

    public async Task<bool> SendVerificationCodeAsync(string chatId, string code)
    {
        var message = $"Your verification code is: {code}";
        await _botClient.SendMessage(chatId, message);
        return true;
    }
    
}