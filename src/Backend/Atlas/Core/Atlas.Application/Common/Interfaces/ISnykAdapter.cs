namespace Atlas.Application.Common.Interfaces;

public interface ISnykAdapter
{
    Task<List<SnykVulnerability>> ScanDependenciesAsync(string projectPath, CancellationToken ct);
    Task<string> FixVulnerabilityAsync(string vulnerabilityId, CancellationToken ct);
}

public record SnykVulnerability(string Id, string PackageName, string Severity, string Title, string FixVersion);

