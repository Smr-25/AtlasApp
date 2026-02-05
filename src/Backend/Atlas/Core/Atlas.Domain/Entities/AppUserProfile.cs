using Atlas.Domain.Abstractions;
using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;
using Atlas.Domain.Exceptions;

namespace Atlas.Domain.Entities;

public class AppUserProfile : BaseEntity, IAggregateRoot
{
    public string JobTitle { get; private set; } = string.Empty; 
    public string? Bio { get; private set; }
    public UserProfession Profession { get; private set; }  

    private readonly List<Integration> _integrations = [];
    private readonly List<Workspace> _workspaces = [];

    public IReadOnlyCollection<Integration> Integrations => _integrations.AsReadOnly();
    public IReadOnlyCollection<Workspace> Workspaces => _workspaces.AsReadOnly();

    private AppUserProfile() { }

    public static AppUserProfile Create(Guid userId, UserProfession profession, string jobTitle)
    {
        return new AppUserProfile
        {
            Id = userId, 
            Profession = profession,
            JobTitle = jobTitle
        };
    }
    
    public void UpdateInfo(string jobTitle, string? bio)
    {
        JobTitle = jobTitle;
        Bio = bio;
        SetModified();
    }
    
    public void AddIntegration(Integration integration)
    {
        if (_integrations.Any(i => i.Provider == integration.Provider && i.Name == integration.Name && !i.IsDeleted))
            throw new BusinessRuleViolationException("Duplicate", "Integration already exists.");

        _integrations.Add(integration);
        SetModified();
    }
    
    public void AddWorkspace(Workspace workspace)
    {
        _workspaces.Add(workspace);
        SetModified();
    }
}
