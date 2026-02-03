using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Services;

public class GreetingService : IGreetingService
{
    public string GetLocalizedGreeting(string userName, int timezoneOffsetMinutes, string languageCode)
    {
        var userLocalTime = DateTime.UtcNow.AddMinutes(timezoneOffsetMinutes);
        var hour = userLocalTime.Hour;
        
        var timeKey = hour switch
        {
            >= 5 and < 12 => "Morning",
            >= 12 and < 18 => "Afternoon",
            >= 18 and < 24 => "Evening",
            _ => "Night"
        };
        return GetMessage(timeKey, languageCode, userName);
    }

    private static string GetMessage(string timeKey, string lang, string name)
    {
        var dictionary = new Dictionary<string, Dictionary<string, string>>
        {
            ["Morning"] = new() { ["en"] = "Good Morning ☀️, {0}", ["az"] = "Sabahın xeyir ☀️, {0}", ["tr"] = "Günaydın ☀️, {0}" },
            ["Afternoon"] = new() { ["en"] = "Good Afternoon 👋, {0}", ["az"] = "Hər vaxtın xeyir 👋, {0}", ["tr"] = "Tünaydın 👋, {0}" },
            ["Evening"] = new() { ["en"] = "Good Evening 🌆, {0}", ["az"] = "Axşamın xeyir 🌆, {0}", ["tr"] = "İyi akşamlar 🌆, {0}" },
            ["Night"] = new() { ["en"] = "Good Night 🌙, {0}", ["az"] = "Gecən xeyir 🌙, {0}", ["tr"] = "İyi geceler 🌙, {0}" },
        };

        var selectedLang = dictionary[timeKey].ContainsKey(lang) ? lang : "en";
        var template = dictionary[timeKey][selectedLang];

        return string.Format(template, name);
    }
}