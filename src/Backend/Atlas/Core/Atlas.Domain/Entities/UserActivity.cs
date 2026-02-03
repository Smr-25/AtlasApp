using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class UserActivity : BaseEntity
{
    public Guid UserId { get; init; }
    public Guid? WorkspaceId { get; init; }
    public string ActionType { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string? MetaData { get; init; }
    
    public static UserActivity Create(Guid userId, string actionType, string description, Guid? workspaceId = null)
    {
        return new UserActivity
        {
            UserId = userId,
            ActionType = actionType,
            Description = description,
            WorkspaceId = workspaceId,
            CreatedAt = DateTime.UtcNow
        };
    }
}