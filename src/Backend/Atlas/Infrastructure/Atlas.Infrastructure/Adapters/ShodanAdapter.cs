using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Adapters;

public class ShodanAdapter(IHttpClientFactory httpClientFactory) : IShodanAdapter
{
    public Task<ShodanHostResult> SearchHostAsync(string ipAddress, CancellationToken ct)
        => Task.FromResult(new ShodanHostResult(ipAddress, null, null, [], []));

    public Task<List<ShodanSearchResult>> SearchQueryAsync(string query, CancellationToken ct)
        => Task.FromResult(new List<ShodanSearchResult>());
}

