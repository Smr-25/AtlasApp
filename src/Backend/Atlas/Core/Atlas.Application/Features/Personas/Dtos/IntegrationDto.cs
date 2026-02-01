namespace Atlas.Application.Features.Personas.Dtos;

public record IntegrationDto(
    Guid Id,
    string Name,
    string Provider 
);