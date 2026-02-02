namespace Atlas.Application.Features.Integrations.Dtos;

public record IntegrationDto(Guid Id, string Name, string Provider, bool IsActive);