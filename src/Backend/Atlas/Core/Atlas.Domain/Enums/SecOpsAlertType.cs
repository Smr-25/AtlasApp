namespace Atlas.Domain.Enums;

public enum SecOpsAlertType
{
    RoguePortDetected = 1,
    ExpiringSslWarning = 2,
    SuspiciousTrafficSpike = 3,
    LeakedKeyDetected = 4,
    AutoPatchSuggestion = 5,
    ZombieProcessDetected = 6,
    VpnDropFailsafe = 7
}

