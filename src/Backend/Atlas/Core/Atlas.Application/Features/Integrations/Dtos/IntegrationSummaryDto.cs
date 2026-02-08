using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Integrations.Dtos;

public record IntegrationSummaryDto(
    Guid Id,
    string Name,
    IntegrationProvider Provider,
    string ProviderIcon,
    IntegrationStatus Status
);