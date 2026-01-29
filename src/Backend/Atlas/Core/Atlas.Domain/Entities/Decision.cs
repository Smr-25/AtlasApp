using System.Text.Json.Serialization;
using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class Decision : BaseEntity
{
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public DecisionStatus Status { get; private set; }
    public DecisionPriority Priority { get; private set; } = DecisionPriority.Medium;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public Guid? GoalId { get; private set; }
    public Guid PersonaId { get; private set; }
    
    [JsonIgnore]
    public Persona Persona { get; private set; } = null!;
    [JsonIgnore]
    public DecisionContext? Context { get; private set; }
    [JsonIgnore]
    public DecisionOutcome? Outcome { get; private set; }
    [JsonIgnore]
    public ICollection<Reflection> Reflections { get; private set; } = new List<Reflection>();
    [JsonIgnore]
    public Goal? RelatedGoal { get; private set; }

    public static Decision Create(Guid personaId, string title, string? description = null,
        DecisionPriority priority = DecisionPriority.Medium, Guid? relatedGoalId = null)
    {
        var decision = new Decision
        {
            PersonaId = personaId,
            Title = title,
            Description = description,
            Priority = priority,
            Status = DecisionStatus.Pending,
            GoalId = relatedGoalId
        };
        return decision;
    }

    public void UpdateStatus(DecisionStatus newStatus)
    {
        Status = newStatus;
    }

    public void Execute()
    {
        Status = DecisionStatus.Executed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Abandon(string? reason = null)
    {
        Status = DecisionStatus.Abandoned;
        ClosedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddReflection(Reflection reflection)
    {
        Reflections.Add(reflection);
    }

    public void SetContext(DecisionContext context)
    {
        Context = context;
    }

    public void SetOutcome(DecisionOutcome outcome)
    {
        Outcome = outcome;
    }
}