using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class TeamMemberFocus : BaseEntity
{
    public Guid TeamMemberId { get; private set; }
    public Guid TeamId { get; private set; }
    public string FocusDescription { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private TeamMemberFocus() { }

    public static TeamMemberFocus Create(Guid teamId, Guid teamMemberId, string focusDescription)
    {
        return new TeamMemberFocus
        {
            TeamId = teamId,
            TeamMemberId = teamMemberId,
            FocusDescription = focusDescription.Trim(),
            IsActive = true
        };
    }

    public void Update(string focusDescription)
    {
        FocusDescription = focusDescription.Trim();
        SetModified();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetModified();
    }
}

