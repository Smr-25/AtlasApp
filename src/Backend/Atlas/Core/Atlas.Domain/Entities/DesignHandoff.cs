using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class DesignHandoff : BaseEntity
{
    public string DesignName { get; private set; } = null!;
    public string? FigmaFileUrl { get; private set; }
    public string? ZeplinScreenUrl { get; private set; }
    public string Status { get; private set; } = null!;
    public string? Notes { get; private set; }
    public Guid DesignerId { get; private set; }
    public Guid? DeveloperId { get; private set; }
    public Guid? WorkspaceId { get; private set; }

    private DesignHandoff() { }

    public static DesignHandoff Create(
        Guid designerId,
        string designName,
        string? figmaFileUrl,
        string? zeplinScreenUrl,
        string? notes,
        Guid? workspaceId = null)
    {
        return new DesignHandoff
        {
            DesignerId = designerId,
            DesignName = designName,
            FigmaFileUrl = figmaFileUrl,
            ZeplinScreenUrl = zeplinScreenUrl,
            Status = "Pending",
            Notes = notes,
            WorkspaceId = workspaceId
        };
    }

    public void AssignDeveloper(Guid developerId)
    {
        DeveloperId = developerId;
        Status = "InProgress";
        SetModified();
    }

    public void MarkDelivered()
    {
        Status = "Delivered";
        SetModified();
    }

    public void MarkCompleted()
    {
        Status = "Completed";
        SetModified();
    }
}

