using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class OutgoingWebhook : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Url { get; private set; } = null!;
    public string? Secret { get; private set; }
    public WebhookEvent[] Events { get; private set; } = [];
    public bool IsActive { get; private set; } = true;
    public int ConsecutiveFailures { get; private set; }
    public DateTime? LastDeliveredAt { get; private set; }
    public Guid? WorkspaceId { get; private set; }

    private OutgoingWebhook() { }

    public static OutgoingWebhook Create(
        Guid userId, string name, string url, string? secret,
        WebhookEvent[] events, Guid? workspaceId = null)
    {
        return new OutgoingWebhook
        {
            UserId = userId,
            Name = name,
            Url = url,
            Secret = secret,
            Events = events,
            WorkspaceId = workspaceId
        };
    }

    public void Update(string name, string url, string? secret, WebhookEvent[] events)
    {
        Name = name;
        Url = url;
        Secret = secret;
        Events = events;
        SetModified();
    }

    public void RecordSuccess()
    {
        ConsecutiveFailures = 0;
        LastDeliveredAt = DateTime.UtcNow;
        SetModified();
    }

    public void RecordFailure()
    {
        ConsecutiveFailures++;
        if (ConsecutiveFailures >= 10) IsActive = false;
        SetModified();
    }

    public void SetActive(bool active)
    {
        IsActive = active;
        if (active) ConsecutiveFailures = 0;
        SetModified();
    }

    public void Delete() => SetDelete();
}

