namespace Atlas.Application.Common.Interfaces;

public interface ISecOpsAgentService
{
    Task<List<RoguePortInfo>> DetectRoguePortsAsync(CancellationToken ct);
    Task<List<ExpiringSslInfo>> WarnExpiringSslAsync(List<string> domains, CancellationToken ct);
    Task<TrafficAnalysisResult> DetectSuspiciousTrafficAsync(string targetUrl, CancellationToken ct);
    Task<List<LeakedKeyInfo>> ScanLeakedKeysAsync(string content, CancellationToken ct);
    Task<List<PatchSuggestion>> SuggestAutoPatchesAsync(string projectPath, CancellationToken ct);
    Task<List<ZombieProcessInfo>> KillZombieProcessesAsync(CancellationToken ct);
    Task<VpnStatusResult> CheckVpnDropAsync(CancellationToken ct);
}

public record RoguePortInfo(int Port, string ProcessName, int ProcessId, string Status);
public record ExpiringSslInfo(string Domain, DateTime ExpiresAt, int DaysRemaining);
public record TrafficAnalysisResult(bool IsSuspicious, int RequestCount, string? OriginCountry, string Summary);
public record LeakedKeyInfo(string KeyType, string Snippet, int LineNumber);
public record PatchSuggestion(string PackageName, string CurrentVersion, string SuggestedVersion, string Severity);
public record ZombieProcessInfo(int ProcessId, string ProcessName, long MemoryMb, string Status);
public record VpnStatusResult(bool IsConnected, string? PublicIp, string? VpnIp, bool IsLeaking);

