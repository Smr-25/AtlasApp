namespace Atlas.Application.Common.Interfaces;

public interface ISecOpsUtilityService
{
    string GenerateHash(string input, string algorithm);
    Task<IpDnsLookupResult> LookupIpDnsAsync(string target, CancellationToken ct);
    string EncodePayload(string input, string encoding);
    PasswordEntropyResult CalculateEntropy(string password);
    Task<SslCheckResult> CheckSslAsync(string hostname, CancellationToken ct);
    Task<List<OpenPortResult>> ScanLocalPortsAsync(string target, int startPort, int endPort, CancellationToken ct);
    Task<string> SpoofMacAsync(string interfaceName, CancellationToken ct);
}

public record IpDnsLookupResult(string Ip, string? Hostname, string? Country, string? Isp, string? Organization);
public record PasswordEntropyResult(double Entropy, string Strength, string EstimatedCrackTime);
public record SslCheckResult(string Subject, string Issuer, DateTime NotBefore, DateTime NotAfter, int DaysRemaining, bool IsValid);
public record OpenPortResult(int Port, string Protocol, string? ServiceName);

