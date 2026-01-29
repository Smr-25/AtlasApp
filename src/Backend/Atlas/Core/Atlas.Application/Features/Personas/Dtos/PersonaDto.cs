namespace Atlas.Application.Features.Personas.Dtos;

public record PersonaDto(
    Guid Id,
    Guid UserId,
    string Name,
    string? Alias,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? DeactivatedAt
);