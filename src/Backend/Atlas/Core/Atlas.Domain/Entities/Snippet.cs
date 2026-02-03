using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class Snippet : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Language { get; set; } = "text";
    public string? Tags { get; set; }
    public bool IsFavorite { get; set; }
    public Guid UserId { get; set; }
    
    public static Snippet Create(string title, string code, string language, List<string> tags, Guid userId)
    {
        return new Snippet
        {
            Id = Guid.NewGuid(),
            Title = title,
            Code = code,
            Language = language,
            Tags = string.Join(",", tags),
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }
}

