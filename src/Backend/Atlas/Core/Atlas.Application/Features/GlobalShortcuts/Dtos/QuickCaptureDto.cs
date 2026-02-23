namespace Atlas.Application.Features.GlobalShortcuts.Dtos;

public record QuickCaptureDto(
    Guid Id,
    string Content,
    string? Title,
    string? Url,
    string Source,
    bool IsSynced,
    DateTimeOffset CreatedAt
);

public record QuickShareResultDto(
    string Channel,
    bool Success,
    string? MessageId
);

public record CalendarEventResultDto(
    string Title,
    DateTime ParsedDateTime,
    bool Created,
    string? EventId
);

