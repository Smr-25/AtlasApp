namespace Atlas.Application.Features.Personas.Dtos;

public record PersonaDetailDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Bio { get; init; }
    public string Type { get; init; } = string.Empty; 
    public bool IsPrimary { get; init; }
    public List<PersonaIntegrationDto> Integrations { get; init; } = [];
    public List<PersonaWorkspaceDto> Workspaces { get; init; } = [];
}

public record PersonaIntegrationDto(Guid Id, string Name, string Provider, bool IsActive);
public record PersonaWorkspaceDto(Guid Id, string Name);