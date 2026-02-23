using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class SharedResource : BaseEntity
{
    public Guid TeamId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public string Url { get; private set; } = null!;
    public ResourceCategory Category { get; private set; }
    public Guid UploadedByUserId { get; private set; }
    public bool IsPinned { get; private set; }

    private SharedResource() { }

    public static SharedResource Create(
        Guid teamId,
        Guid uploadedByUserId,
        string title,
        string url,
        ResourceCategory category,
        string? description = null)
    {
        return new SharedResource
        {
            TeamId = teamId,
            UploadedByUserId = uploadedByUserId,
            Title = title,
            Url = url,
            Category = category,
            Description = description,
            IsPinned = false
        };
    }

    public void Update(string title, string url, ResourceCategory category, string? description)
    {
        Title = title;
        Url = url;
        Category = category;
        Description = description;
        SetModified();
    }

    public void TogglePin()
    {
        IsPinned = !IsPinned;
        SetModified();
    }
}

