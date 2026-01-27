using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class Goal : BaseEntity
{
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public GoalStatus Status { get; private set; }
    public int Priority { get; private set; } = 5;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public int ProgressPercentage { get; private set; } = 0;
    public Guid PersonaId { get; private set; }
    public Persona Persona { get; private set; } = null!;
    public ICollection<Decision> RelatedDecisions { get; private set; } = new List<Decision>();

    public static Goal Create(Guid personaId, string title, string? description = null,
        int priority = 5, DateTime? dueDate = null)
    {
        var goal = new Goal
        {
            Title = title,
            Description = description,
            Priority = priority,
            DueDate = dueDate,
            Status = GoalStatus.Active,
            Persona = Persona.Create(personaId, "Goal Persona")
        };
        return goal;
    }

    public void UpdateProgress(int percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentOutOfRangeException(nameof(percentage), "Progress percentage must be between 0 and 100.");

        ProgressPercentage = percentage;

        if (ProgressPercentage == 100)
            Complete();
    }

    public void Complete()
    {
        Status = GoalStatus.Completed;
    }

    public void Pause()
    {
        Status = GoalStatus.Paused;
    }

    public void Resume()
    {
        Status = GoalStatus.Active;
    }

    public void Abandon(string? reason = null)
    {
        Status = GoalStatus.Abandoned;
    }
}