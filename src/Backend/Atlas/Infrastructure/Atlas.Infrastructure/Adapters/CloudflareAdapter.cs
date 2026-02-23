using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Adapters;

public class CloudflareAdapter(IHttpClientFactory httpClientFactory) : ICloudflareAdapter
{
    public Task<string> EnableUnderAttackModeAsync(string zoneId, CancellationToken ct)
        => Task.FromResult($"Under Attack mode enabled for zone {zoneId}");

    public Task<string> DisableUnderAttackModeAsync(string zoneId, CancellationToken ct)
        => Task.FromResult($"Under Attack mode disabled for zone {zoneId}");

    public Task<List<WafRuleResult>> GetWafRulesAsync(string zoneId, CancellationToken ct)
        => Task.FromResult(new List<WafRuleResult>());

    public Task<string> BlockIpAsync(string zoneId, string ipAddress, CancellationToken ct)
        => Task.FromResult($"IP {ipAddress} blocked on zone {zoneId}");

    public Task<string> UnblockIpAsync(string zoneId, string ipAddress, CancellationToken ct)
        => Task.FromResult($"IP {ipAddress} unblocked on zone {zoneId}");
}

