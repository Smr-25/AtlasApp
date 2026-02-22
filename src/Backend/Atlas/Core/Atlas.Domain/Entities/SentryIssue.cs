using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class SentryIssue : BaseEntity
{
    public string ExternalId { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Culprit { get; private set; } = null!;
    public SentryIssueLevel Level { get; private set; }
    public int LineNumber { get; private set; }
    public string FileName { get; private set; } = null!;
    public string? StackTrace { get; private set; }
    public int EventCount { get; private set; }
    public bool IsResolved { get; private set; }
    public Guid IntegrationId { get; private set; }
    public Guid UserId { get; private set; }

    private SentryIssue() { }

    public static SentryIssue Create(
        Guid userId,
        Guid integrationId,
        string externalId,
        string title,
        string culprit,
        SentryIssueLevel level,
        int lineNumber,
        string fileName,
        string? stackTrace,
        int eventCount)
    {
        return new SentryIssue
        {
            UserId = userId,
            IntegrationId = integrationId,
            ExternalId = externalId,
            Title = title,
            Culprit = culprit,
            Level = level,
            LineNumber = lineNumber,
            FileName = fileName,
            StackTrace = stackTrace,
            EventCount = eventCount,
            IsResolved = false
        };
    }

    public void Resolve()
    {
        IsResolved = true;
        SetModified();
    }

    public void UpdateEventCount(int count)
    {
        EventCount = count;
        SetModified();
    }
}

