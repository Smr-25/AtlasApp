using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class SupportTicket : BaseEntity
{
    public Guid UserId { get; private set; }
    public FeedbackType Type { get; private set; }
    public FeedbackStatus Status { get; private set; }
    public string Subject { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public string? PageUrl { get; private set; }
    public string? BrowserInfo { get; private set; }
    public string? AdminReply { get; private set; }
    public DateTime? RepliedAt { get; private set; }

    private SupportTicket() { }

    public static SupportTicket Create(
        Guid userId, FeedbackType type, string subject, string body,
        string? pageUrl = null, string? browserInfo = null)
    {
        return new SupportTicket
        {
            UserId = userId,
            Type = type,
            Status = FeedbackStatus.Open,
            Subject = subject,
            Body = body,
            PageUrl = pageUrl,
            BrowserInfo = browserInfo
        };
    }

    public void Reply(string reply)
    {
        AdminReply = reply;
        RepliedAt = DateTime.UtcNow;
        Status = FeedbackStatus.Resolved;
        SetModified();
    }

    public void Close()
    {
        Status = FeedbackStatus.Closed;
        SetModified();
    }

    public void Delete() => SetDelete();
}

