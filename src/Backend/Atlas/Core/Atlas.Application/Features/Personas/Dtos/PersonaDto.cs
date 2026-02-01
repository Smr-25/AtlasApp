using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Personas.Dtos;

public record PersonaDto(
    Guid Id,
    string Name,
    string Bio,
    PersonaType Type,
    bool IsPrimary,
    List<IntegrationDto> Integrations 
);