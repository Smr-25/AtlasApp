using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Integrations.Dtos;

public class IntegrationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public IntegrationProvider Provider { get; set; }
    public IntegrationStatus Status { get; set; }
    public IntegrationScope Scope { get; set; }
    public string? MetadataJson { get; set; }
}
