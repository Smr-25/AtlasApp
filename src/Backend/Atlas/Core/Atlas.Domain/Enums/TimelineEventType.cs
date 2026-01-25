namespace Atlas.Domain.Enums;

public enum TimelineEventType
{
    StateChange = 1,
    DecisionMade,
    DecisionExecuted,
    DecisionAbandoned,
    OutcomeRecorded,
    ReflectionAdded,
    GoalCreated,
    GoalCompleted,
    GoalAbandoned,
    ConstraintAdded,
    PhaseChanged
}