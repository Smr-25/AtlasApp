using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;
using Atlas.Domain.Exceptions;

namespace Atlas.Domain.Entities;

public partial class Workspace : BaseEntity
{
    public string Name { get; private set; } = null!; 
    public string? Description { get; private set; }
    public bool IsDefault { get; private set; }
    public string? LocalFolderPath { get; private set; }
    public bool IsShared { get; private set; }

    public Guid UserProfileId { get; private set; } 
    
    private readonly List<WorkspaceIntegration> _workspaceIntegrations = [];
    public IReadOnlyCollection<WorkspaceIntegration> WorkspaceIntegrations => _workspaceIntegrations.AsReadOnly();

    private readonly List<WorkspaceMember> _members = [];
    public IReadOnlyCollection<WorkspaceMember> Members => _members.AsReadOnly();
    
    private Workspace() { }

    public static Workspace Create(string name, Guid userProfileId, bool isDefault = false, string? localFolderPath = null)
    {
        var workspace = new Workspace
        {
            Name = name,
            UserProfileId = userProfileId,
            IsDefault = isDefault,
            LocalFolderPath = localFolderPath,
            IsShared = false
        };
        
        workspace._members.Add(WorkspaceMember.Create(workspace.Id, userProfileId, WorkspaceMemberRole.Owner));
        
        return workspace;
    }
    
    public void UpdateDetails(string name, string? description)
    {
        Name = name;
        Description = description;
        SetModified();
    }

    public void SetLocalFolderPath(string? path)
    {
        LocalFolderPath = path;
        SetModified();
    }

    public void SetShared(bool isShared)
    {
        IsShared = isShared;
        SetModified();
    }

    public void SetDefault(bool isDefault)
    {
        IsDefault = isDefault;
        SetModified();
    }
    
    public void AddMember(Guid userId, WorkspaceMemberRole role = WorkspaceMemberRole.Viewer)
    {
        if (_members.Any(m => m.UserId == userId && !m.IsDeleted))
            throw new BusinessRuleViolationException("Duplicate", "User is already a member of this workspace.");
        
        _members.Add(WorkspaceMember.Create(Id, userId, role));
        
        if (!IsShared)
            SetShared(true);
        
        SetModified();
    }
    
    public void RemoveMember(Guid userId)
    {
        if (userId == UserProfileId)
            throw new BusinessRuleViolationException("OwnerRemoval", "Cannot remove the workspace owner.");
        
        var member = _members.FirstOrDefault(m => m.UserId == userId && !m.IsDeleted);
        if (member == null)
            throw new BusinessRuleViolationException("NotFound", "Member not found in this workspace.");
        
        member.Remove();
        SetModified();
    }
    
    public void ChangeMemberRole(Guid userId, WorkspaceMemberRole newRole)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId && !m.IsDeleted);
        if (member == null)
            throw new BusinessRuleViolationException("NotFound", "Member not found in this workspace.");
        
        if (newRole == WorkspaceMemberRole.Owner)
            throw new BusinessRuleViolationException("InvalidRole", "Cannot assign Owner role. Transfer ownership instead.");
        
        member.ChangeRole(newRole);
        SetModified();
    }
    
    public void Delete()
    { 
        SetDelete();
    }
}