using Atlas.Application.Common.Models;
using Atlas.Domain.Enums;

namespace Atlas.Application.Common.Interfaces;

public interface IIntegrationAdapter
{
    IntegrationProvider Provider { get; }
    Task<List<ExternalResourceDto>> SearchResourcesAsync(string accessToken, string query, CancellationToken cancellationToken);
    Task<ExternalResourceDto> GetResourceDetailsAsync(string accessToken, string resourceId, CancellationToken cancellationToken);
}