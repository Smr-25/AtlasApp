using Atlas.Domain.Entities.Common;
using Atlas.Domain.Exceptions;

namespace Atlas.Domain.Entities;

public class TeamObjective : BaseEntity
{
    public Guid TeamId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime? Deadline { get; private set; }
    public bool IsActive { get; private set; }

    private TeamObjective() { }

    public static TeamObjective Create(Guid teamId, string title, string? description = null, DateTime? deadline = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new InvalidEntityStateException(nameof(TeamObjective), nameof(Title), "Objective title cannot be empty.");

        return new TeamObjective
        {
            TeamId = teamId,
            Title = title.Trim(),
            Description = description?.Trim(),
            Deadline = deadline,
            IsActive = true
        };
    }

    public void Update(string title, string? description, DateTime? deadline)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new InvalidEntityStateException(nameof(TeamObjective), nameof(Title), "Objective title cannot be empty.");

        Title = title.Trim();
        Description = description?.Trim();
        Deadline = deadline;
        SetModified();
    }

    public void Complete()
    {
        IsActive = false;
        SetModified();
    }

    public void Reactivate()
    {
        IsActive = true;
        SetModified();
    }
}

