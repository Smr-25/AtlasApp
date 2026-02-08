using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Integrations.Dtos;

public record IntegrationDetailDto(
    Guid Id,
    string Name,
    IntegrationProvider Provider,
    IntegrationStatus Status,
    string ProviderName,
    string StatusName,
    DateTime? TokenExpiresAt,
    bool IsExpired,
    int WorkspaceCount,
    string? MetadataJson,
    DateTime CreatedAt,
    DateTime? ModifiedAt
);