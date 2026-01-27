using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class PersonaTimelineEvent : BaseEntity
{
    public TimelineEventType EventType { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid? RelatedEntityId { get; private set; }
    public string? RelatedEntityType { get; private set; }
    public string? Metadata { get; private set; }
    public DateTime OccurredAt { get; private set; } = DateTime.UtcNow;
    public Guid PersonaId { get; private set; }
    public Persona? Persona { get; private set; }

    public static PersonaTimelineEvent CreateStateChange(Guid personaId, LifePhase oldPhase, LifePhase newPhase)
    {
        var timelineEvent = new PersonaTimelineEvent
        {
            PersonaId = personaId,
            EventType = TimelineEventType.StateChange,
            Title = $"Life Phase changed from {oldPhase} to {newPhase}",
            Description = $"The persona has transitioned from the {oldPhase} phase to the {newPhase} phase.",
            RelatedEntityType = nameof(LifePhase),
            RelatedEntityId = null,
            Metadata = $"{{ \"OldPhase\": \"{oldPhase}\", \"NewPhase\": \"{newPhase}\" }}",
            OccurredAt = DateTime.UtcNow
        };
        return timelineEvent;
    }

    public static PersonaTimelineEvent CreateDecisionMade(Guid personaId, Decision decision)
    {
        var decisionEvent = new PersonaTimelineEvent
        {
            PersonaId = personaId,
            EventType = TimelineEventType.DecisionMade,
            Title = $"Decision Made: {decision.Title}",
            Description = decision.Description,
            RelatedEntityType = nameof(Decision),
            RelatedEntityId = decision.Id,
            Metadata = null,
            OccurredAt = decision.CreatedAt
        };
        return decisionEvent;
    }

    public static PersonaTimelineEvent CreateDecisionOutcome(Guid personaId, Decision decision, DecisionOutcome outcome)
    {
        var outcomeEvent = new PersonaTimelineEvent
        {
            PersonaId = personaId,
            EventType = TimelineEventType.DecisionExecuted,
            Title = $"Decision Outcome for: {decision.Title}",
            Description = outcome.Description,
            RelatedEntityType = nameof(Decision),
            RelatedEntityId = decision.Id,
            Metadata = $"{{ \"Outcome\": \"{outcome}\" }}",
            OccurredAt = outcome.RecordedAt
        };
        return outcomeEvent;
    }

    public static PersonaTimelineEvent CreateReflection(Guid personaId, Reflection reflection)
    {
        var reflectionEvent = new PersonaTimelineEvent
        {
            PersonaId = personaId,
            EventType = TimelineEventType.ReflectionAdded,
            Title = $"Reflection Added",
            Description = reflection.Content,
            RelatedEntityType = nameof(Reflection),
            RelatedEntityId = reflection.Id,
            Metadata = null,
            OccurredAt = reflection.CreatedAt
        };
        return reflectionEvent;
    }

    public static PersonaTimelineEvent CreateGoalCreated(Guid personaId, Goal goal)
    {
        var goalEvent = new PersonaTimelineEvent
        {
            PersonaId = personaId,
            EventType = TimelineEventType.GoalCreated,
            Title = $"Goal Created: {goal.Title}",
            Description = goal.Description,
            RelatedEntityType = nameof(Goal),
            RelatedEntityId = goal.Id,
            Metadata = null,
            OccurredAt = goal.CreatedAt
        };
        return goalEvent;
    }

    public static PersonaTimelineEvent CreateGoalCompleted(Guid personaId, Goal goal)
    {
        var goalEvent = new PersonaTimelineEvent
        {
            PersonaId = personaId,
            EventType = TimelineEventType.GoalCompleted,
            Title = $"Goal Completed: {goal.Title}",
            Description = goal.Description,
            RelatedEntityType = nameof(Goal),
            RelatedEntityId = goal.Id,  
            Metadata = null,
            OccurredAt = goal.CompletedAt ?? DateTime.UtcNow
        };
        return goalEvent;
    }
}