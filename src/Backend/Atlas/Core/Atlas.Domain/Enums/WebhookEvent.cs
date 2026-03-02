namespace Atlas.Domain.Enums;

public enum WebhookEvent
{
    AlertFired = 1,
    PrOpened = 2,
    PrApproved = 3,
    DeploymentCompleted = 4,
    SecurityThreat = 5,
    FocusCompleted = 6,
    ScriptCompleted = 7,
    MemberJoined = 8,
    BudgetBleed = 9,
    Custom = 99
}

