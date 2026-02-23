using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Adapters;

public class GA4Adapter(IHttpClientFactory httpClientFactory) : IGA4Adapter
{
    public Task<GA4RealtimeResult> GetRealtimeUsersAsync(string propertyId, CancellationToken ct)
        => Task.FromResult(new GA4RealtimeResult(0, []));

    public Task<List<GA4PageView>> GetTopPagesAsync(string propertyId, DateTime from, DateTime to, CancellationToken ct)
        => Task.FromResult(new List<GA4PageView>());
}

