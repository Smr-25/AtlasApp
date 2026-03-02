using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class WorkspaceMember : BaseEntity
{
    public Guid WorkspaceId { get; private set; }
    public Workspace Workspace { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public WorkspaceMemberRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }

    private WorkspaceMember() { }

    public static WorkspaceMember Create(Guid workspaceId, Guid userId, WorkspaceMemberRole role)
    {
        return new WorkspaceMember
        {
            WorkspaceId = workspaceId,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };
    }

    public void ChangeRole(WorkspaceMemberRole newRole)
    {
        Role = newRole;
        SetModified();
    }

    public void Remove()
    {
        SetDelete();
    }
}

