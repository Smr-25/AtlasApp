using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Integrations.Dtos;

public record IntegrationDto(
    Guid Id,
    string Name,
    IntegrationProvider Provider,
    IntegrationStatus Status,
    string? MetadataJson 
);