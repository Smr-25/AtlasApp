namespace Atlas.Application.Dtos.Users.State;

public record PersonaStateReturnDto(
    string CurrentPhase,
    string MentalLoadLevel,
    DateTime LastUpdatedAt
);