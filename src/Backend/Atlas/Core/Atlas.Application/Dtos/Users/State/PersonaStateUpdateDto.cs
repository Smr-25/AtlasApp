namespace Atlas.Application.Dtos.Users.State;

public record PersonaStateUpdateDto(
    string CurrentPhase,
    string MentalLoadLevel
);