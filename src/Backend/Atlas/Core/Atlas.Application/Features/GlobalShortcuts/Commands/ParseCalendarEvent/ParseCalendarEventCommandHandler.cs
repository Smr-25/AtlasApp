using System.Text.Json;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.GlobalShortcuts.Dtos;
using MediatR;

namespace Atlas.Application.Features.GlobalShortcuts.Commands.ParseCalendarEvent;

public class ParseCalendarEventCommandHandler(
    IAiService aiService,
    ICurrentUserService currentUserService)
    : IRequestHandler<ParseCalendarEventCommand, CalendarEventResultDto>
{
    public async Task<CalendarEventResultDto> Handle(ParseCalendarEventCommand request, CancellationToken cancellationToken)
    {
        currentUserService.GetRequiredUserId();

        var systemPrompt = "You are a date/time parser. Extract the meeting title and exact date/time from the text. " +
                          "Return ONLY a JSON object: {\"title\": \"...\", \"dateTime\": \"yyyy-MM-ddTHH:mm:ss\"}. " +
                          "If no date found, use tomorrow 10:00. Today is " + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".";

        var aiResult = await aiService.GenerateResponseAsync(systemPrompt, request.Text, cancellationToken);

        try
        {
            var parsed = JsonSerializer.Deserialize<ParsedEvent>(aiResult,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (parsed != null && DateTime.TryParse(parsed.DateTime, out var dt))
            {
                return new CalendarEventResultDto(parsed.Title ?? "Meeting", dt, true, null);
            }
        }
        catch
        {
        }

        return new CalendarEventResultDto("Meeting", DateTime.UtcNow.AddDays(1).Date.AddHours(10), false, null);
    }

    private record ParsedEvent(string? Title, string? DateTime);
}
