using Atlas.Domain.Enums;

namespace Atlas.Application.Features.PersonaStates.Dtos;

public record PersonaStateHistoryDto(
    Guid Id,
    LifePhase Phase,
    MentalLoadLevel MentalLoad,
    int EnergyLevel,
    int FocusLevel,
    string? Note,
    DateTime StartedAt,
    DateTime EndedAt,
    int DurationDays
);