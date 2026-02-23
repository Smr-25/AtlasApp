namespace Atlas.Application.Common.Interfaces;

public interface IShodanAdapter
{
    Task<ShodanHostResult> SearchHostAsync(string ipAddress, CancellationToken ct);
    Task<List<ShodanSearchResult>> SearchQueryAsync(string query, CancellationToken ct);
}

public record ShodanHostResult(string Ip, string? Organization, string? Os, List<int> OpenPorts, List<string> Hostnames);
public record ShodanSearchResult(string Ip, int Port, string? Banner, string? Organization);

