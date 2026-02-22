namespace Atlas.Application.Features.Sentry.Dtos;

public record SentryIssueDto(
    string Id,
    string Title,
    string Culprit,
    string Level,
    int EventCount,
    string ShortId,
    DateTime FirstSeen,
    DateTime LastSeen,
    bool IsResolved);

public record SentryIssueDetailDto(
    string Id,
    string Title,
    string Culprit,
    string Level,
    int LineNumber,
    string FileName,
    string StackTrace,
    int EventCount,
    string ProjectName,
    DateTime FirstSeen,
    DateTime LastSeen);

