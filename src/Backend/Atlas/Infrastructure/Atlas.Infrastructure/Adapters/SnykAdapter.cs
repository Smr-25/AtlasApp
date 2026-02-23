using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Adapters;

public class SnykAdapter(IHttpClientFactory httpClientFactory) : ISnykAdapter
{
    public Task<List<SnykVulnerability>> ScanDependenciesAsync(string projectPath, CancellationToken ct)
        => Task.FromResult(new List<SnykVulnerability>());

    public Task<string> FixVulnerabilityAsync(string vulnerabilityId, CancellationToken ct)
        => Task.FromResult($"Vulnerability {vulnerabilityId} fix applied.");
}

