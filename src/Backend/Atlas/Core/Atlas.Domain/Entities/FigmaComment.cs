using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class FigmaComment : BaseEntity
{
    public string FileKey { get; private set; } = null!;
    public string CommentId { get; private set; } = null!;
    public string AuthorName { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public bool IsResolved { get; private set; }
    public DateTime PostedAt { get; private set; }
    public Guid IntegrationId { get; private set; }
    public Guid UserId { get; private set; }

    private FigmaComment() { }

    public static FigmaComment Create(
        Guid userId,
        Guid integrationId,
        string fileKey,
        string commentId,
        string authorName,
        string message,
        DateTime postedAt)
    {
        return new FigmaComment
        {
            UserId = userId,
            IntegrationId = integrationId,
            FileKey = fileKey,
            CommentId = commentId,
            AuthorName = authorName,
            Message = message,
            PostedAt = postedAt,
            IsResolved = false
        };
    }

    public void Resolve()
    {
        IsResolved = true;
        SetModified();
    }
}

