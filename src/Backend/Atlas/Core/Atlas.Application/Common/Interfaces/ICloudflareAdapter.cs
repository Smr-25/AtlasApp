namespace Atlas.Application.Common.Interfaces;

public interface ICloudflareAdapter
{
    Task<string> EnableUnderAttackModeAsync(string zoneId, CancellationToken ct);
    Task<string> DisableUnderAttackModeAsync(string zoneId, CancellationToken ct);
    Task<List<WafRuleResult>> GetWafRulesAsync(string zoneId, CancellationToken ct);
    Task<string> BlockIpAsync(string zoneId, string ipAddress, CancellationToken ct);
    Task<string> UnblockIpAsync(string zoneId, string ipAddress, CancellationToken ct);
}

public record WafRuleResult(string Id, string Description, bool Enabled);

