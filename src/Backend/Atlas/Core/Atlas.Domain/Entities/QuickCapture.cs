using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;
using Atlas.Domain.Exceptions;

namespace Atlas.Domain.Entities;

public class QuickCapture : BaseEntity
{
    public string Content { get; private set; } = null!;
    public string? Title { get; private set; }
    public string? Url { get; private set; }
    public QuickCaptureSource Source { get; private set; }
    public bool IsSynced { get; private set; }
    public string? ExternalId { get; private set; }
    public Guid UserId { get; private set; }

    private QuickCapture() { }

    public static QuickCapture Create(
        Guid userId,
        string content,
        QuickCaptureSource source,
        string? title = null,
        string? url = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidEntityStateException(nameof(QuickCapture), nameof(Content), "Capture content cannot be empty.");

        return new QuickCapture
        {
            UserId = userId,
            Content = content.Trim(),
            Source = source,
            Title = title?.Trim(),
            Url = url?.Trim(),
            IsSynced = false
        };
    }

    public void MarkSynced(string externalId)
    {
        IsSynced = true;
        ExternalId = externalId;
        SetModified();
    }
}

