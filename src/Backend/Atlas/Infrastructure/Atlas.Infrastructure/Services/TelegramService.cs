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
        var message = $"🔐 *ATLAS Verification Code*\n\nYour code: `{code}`\n\n⏰ Expires in 15 minutes.\n⚠️ Do not share this code with anyone.";
        await _botClient.SendMessage(chatId, message, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        return true;
    }

    public async Task<bool> SendMessageAsync(string chatId, string message)
    {
        await _botClient.SendMessage(chatId, message, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        return true;
    }
}