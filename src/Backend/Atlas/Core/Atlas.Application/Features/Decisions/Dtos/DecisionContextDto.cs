using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Decisions.Dtos;

public record DecisionContextDto(
    LifePhase PhaseAtDecision,
    MentalLoadLevel MentalLoadAtDecision,
    int EnergyLevelAtDecision,
    int FocusLevelAtDecision,
    int ActiveGoalCount,
    int ActiveDecisionCount,
    string? AdditionalNotes,
    DateTime CapturedAt
);