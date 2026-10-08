namespace Atlas.Application.Common.Interfaces;
public interface ISystemTools
{
    string GenerateHash(string input, string algorithm);
    Task<string[]> LookupIpDnsAsync(string target, CancellationToken ct);
    Task<SslCheckResult> CheckSslAsync(string hostname, CancellationToken ct);
}
public record SslCheckResult(string Subject, string Issuer, DateTime NotBefore, DateTime NotAfter, int DaysRemaining, bool IsValid);
