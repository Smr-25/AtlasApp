namespace Atlas.Application.Common.Interfaces;

public interface IGreetingService
{
    string GetLocalizedGreeting(string userName, int timezoneOffsetMinutes, string languageCode);
}