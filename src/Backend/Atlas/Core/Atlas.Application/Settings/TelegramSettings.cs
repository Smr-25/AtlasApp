namespace Atlas.Application.Settings;

public class TelegramSettings
{
    public const string SectionName = "ThirdPartyServices:TelegramSettings";
    public string BotToken { get; set; } = null!;
    public string BotUsername { get; set; } = null!;
}