using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class DecisionContext : BaseEntity
{
    public LifePhase PhaseAtDecision { get; private set; }
    public MentalLoadLevel MentalLoadAtDecision { get; private set; }
    public int EnergyLevelAtDecision { get; private set; }
    public int FocusLevelAtDecision { get; private set; }
    public int ActiveGoalCount { get; private set; }
    public int ActiveDecisionCount { get; private set; }
    public string? AdditionalNotes { get; private set; }
    public DateTime CapturedAt { get; private set; } = DateTime.UtcNow;
    public Guid DecisionId { get; private set; }
    public Decision Decision { get; private set; } = null!;

    public static DecisionContext CaptureFrom(Guid decisionId, PersonaState currentState,
        int activeGoalCount, int activeDecisionCount, string? notes = null)
    {
        var context = new DecisionContext
        {
            DecisionId = decisionId,
            PhaseAtDecision = currentState.CurrentPhase,
            MentalLoadAtDecision = currentState.MentalLoad,
            EnergyLevelAtDecision = currentState.EnergyLevel,
            FocusLevelAtDecision = currentState.FocusLevel,
            ActiveGoalCount = activeGoalCount,
            ActiveDecisionCount = activeDecisionCount,
            AdditionalNotes = notes,
            CapturedAt = DateTime.UtcNow
        };
        return context;
    }

}