namespace Atlas.Application.Features.Focus.Dtos;

public record FocusHistoryDto(
    Guid Id,
    int DurationMinutes,
    int BreakDurationMinutes,
    string Tag,
    string SessionType,
    string Status,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    Guid? WorkspaceId
);

