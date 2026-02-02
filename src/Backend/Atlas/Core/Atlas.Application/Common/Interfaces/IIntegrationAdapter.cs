using Atlas.Application.Common.Models;
using Atlas.Domain.Enums;

namespace Atlas.Application.Common.Interfaces;

public interface IIntegrationAdapter
{
    IntegrationProvider Provider { get; }
    
    Task<List<ExternalResourceDto>> GetResourcesAsync(string accessToken, CancellationToken cancellationToken);
}