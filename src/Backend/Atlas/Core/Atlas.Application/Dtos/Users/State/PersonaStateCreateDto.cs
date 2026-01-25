namespace Atlas.Application.Dtos.Users.State;

public record PersonaStateCreateDto(
    string CurrentPhase,
    string MentalLoadLevel
);    