using Atlas.Domain.Enums;

namespace Atlas.Application.Features.PersonaStates.Dtos;

public record PersonaStateDto(
    Guid Id,
    LifePhase CurrentPhase,
    MentalLoadLevel MentalLoad,
    int EnergyLevel,
    int FocusLevel,
    string? Note,
    DateTime LastUpdatedAt
);