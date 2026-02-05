using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class Workspace : BaseEntity
{
    public string Name { get; private set; } = null!; 
    public string? Description { get; private set; }
    public bool IsDefault { get; private set; } 

    public Guid UserProfileId { get; private set; } 
    
    private Workspace() { }

    public static Workspace Create(string name, Guid userProfileId, bool isDefault = false)
    {
        return new Workspace
        {
            Name = name,
            UserProfileId = userProfileId,
            IsDefault = isDefault
        };
    }
}