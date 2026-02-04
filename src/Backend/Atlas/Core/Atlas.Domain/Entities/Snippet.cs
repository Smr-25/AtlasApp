using Atlas.Domain.Entities.Common;
using Atlas.Domain.Exceptions;

namespace Atlas.Domain.Entities;

public class Snippet : BaseEntity
{
    public string Title { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public string Language { get; private set; } = "text";
    public string? Tags { get; private set; }
    public bool IsFavorite { get; private set; }
    public Guid UserId { get; private set; }

    private Snippet() { }

    public static Snippet Create(string title, string code, string language,
        List<string> tags, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new InvalidEntityStateException(nameof(Snippet), nameof(Title),
                "Snippet title cannot be empty.");

        if (title.Length > 200)
            throw new InvalidEntityStateException(nameof(Snippet), nameof(Title),
                "Snippet title cannot exceed 200 characters.");

        if (string.IsNullOrWhiteSpace(code))
            throw new InvalidEntityStateException(nameof(Snippet), nameof(Code),
                "Code cannot be empty.");

        if (userId == Guid.Empty)
            throw new InvalidEntityStateException(nameof(Snippet), nameof(UserId),
                "User ID cannot be empty.");

        return new Snippet
        {
            Title = title.Trim(),
            Code = code,
            Language = string.IsNullOrWhiteSpace(language) ? "text" : language.ToLowerInvariant(),
            Tags = tags?.Count > 0 ? string.Join(",", tags.Select(t => t.Trim())) : null,
            UserId = userId,
            IsFavorite = false
        };
    }

    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new InvalidEntityStateException(nameof(Snippet), nameof(Title),
                "Snippet title cannot be empty.");

        if (title.Length > 200)
            throw new InvalidEntityStateException(nameof(Snippet), nameof(Title),
                "Snippet title cannot exceed 200 characters.");

        Title = title.Trim();
        SetModified();
    }

    public void UpdateCode(string code, string? language = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new InvalidEntityStateException(nameof(Snippet), nameof(Code),
                "Code cannot be empty.");

        Code = code;
        if (!string.IsNullOrWhiteSpace(language))
            Language = language.ToLowerInvariant();
        SetModified();
    }

    public void ToggleFavorite()
    {
        IsFavorite = !IsFavorite;
        SetModified();
    }

    public void UpdateTags(List<string> tags)
    {
        Tags = tags?.Count > 0 ? string.Join(",", tags.Select(t => t.Trim())) : null;
        SetModified();
    }
}

