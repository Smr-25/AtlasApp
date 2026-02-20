using Atlas.Domain.Entities.Common;

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

    
    private Workspace() { }

    public static Workspace Create(string name, Guid userProfileId, bool isDefault = false, string? localFolderPath = null)
    {
        return new Workspace
        {
            Name = name,
            UserProfileId = userProfileId,
            IsDefault = isDefault,
            LocalFolderPath = localFolderPath,
            IsShared = false
        };
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
    
    public void Delete()
    { 
        SetDelete();
    }
}