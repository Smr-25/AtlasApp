using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class OmniFeedItem : BaseEntity
{
    public Guid TeamId { get; private set; }
    public Guid? UserId { get; private set; }
    public OmniFeedSource Source { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Body { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? MetadataJson { get; private set; }
    public bool IsRead { get; private set; }
    public string? Emoji { get; private set; }

    private OmniFeedItem() { }

    public static OmniFeedItem Create(
        Guid teamId,
        OmniFeedSource source,
        string title,
        string? body = null,
        Guid? userId = null,
        string? metadataJson = null)
    {
        return new OmniFeedItem
        {
            TeamId = teamId,
            UserId = userId,
            Source = source,
            Title = title,
            Body = body,
            Timestamp = DateTime.UtcNow,
            MetadataJson = metadataJson,
            IsRead = false
        };
    }

    public void MarkAsRead()
    {
        IsRead = true;
        SetModified();
    }

    public void AddEmoji(string emoji)
    {
        Emoji = emoji;
        SetModified();
    }
}

