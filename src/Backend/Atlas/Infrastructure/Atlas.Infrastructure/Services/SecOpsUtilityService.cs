using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Services;

public class SecOpsUtilityService(IScriptRunnerService scriptRunner) : ISecOpsUtilityService
{
    public string GenerateHash(string input, string algorithm)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = algorithm.ToUpperInvariant() switch
        {
            "MD5" => MD5.HashData(bytes),
            "SHA1" => SHA1.HashData(bytes),
            "SHA256" => SHA256.HashData(bytes),
            "SHA384" => SHA384.HashData(bytes),
            "SHA512" => SHA512.HashData(bytes),
            _ => SHA256.HashData(bytes)
        };
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    public async Task<IpDnsLookupResult> LookupIpDnsAsync(string target, CancellationToken ct)
    {
        var result = await scriptRunner.ExecuteAsync("nslookup", target, ".", ct);
        return new IpDnsLookupResult(target, null, null, null, result);
    }

    public string EncodePayload(string input, string encoding)
    {
        return encoding.ToUpperInvariant() switch
        {
            "BASE64" => Convert.ToBase64String(Encoding.UTF8.GetBytes(input)),
            "URL" => Uri.EscapeDataString(input),
            "HEX" => Convert.ToHexString(Encoding.UTF8.GetBytes(input)).ToLowerInvariant(),
            _ => Convert.ToBase64String(Encoding.UTF8.GetBytes(input))
        };
    }

    public PasswordEntropyResult CalculateEntropy(string password)
    {
        var charsetSize = 0;
        if (password.Any(char.IsLower)) charsetSize += 26;
        if (password.Any(char.IsUpper)) charsetSize += 26;
        if (password.Any(char.IsDigit)) charsetSize += 10;
        if (password.Any(c => !char.IsLetterOrDigit(c))) charsetSize += 32;

        var entropy = password.Length * Math.Log2(Math.Max(charsetSize, 1));
        var strength = entropy switch
        {
            >= 80 => "Very Strong",
            >= 60 => "Strong",
            >= 40 => "Moderate",
            >= 28 => "Weak",
            _ => "Very Weak"
        };

        var crackTime = entropy switch
        {
            >= 80 => "centuries",
            >= 60 => "years",
            >= 40 => "months",
            >= 28 => "days",
            _ => "seconds"
        };

        return new PasswordEntropyResult(Math.Round(entropy, 2), strength, crackTime);
    }

    public async Task<SslCheckResult> CheckSslAsync(string hostname, CancellationToken ct)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(hostname, 443, ct);
        await using var sslStream = new SslStream(client.GetStream(), false);
        await sslStream.AuthenticateAsClientAsync(hostname);

        var cert = sslStream.RemoteCertificate;
        if (cert == null)
            return new SslCheckResult(hostname, "Unknown", DateTime.MinValue, DateTime.MinValue, 0, false);

        var cert2 = new System.Security.Cryptography.X509Certificates.X509Certificate2(cert);
        var daysRemaining = (cert2.NotAfter - DateTime.UtcNow).Days;

        return new SslCheckResult(
            cert2.Subject,
            cert2.Issuer,
            cert2.NotBefore,
            cert2.NotAfter,
            daysRemaining,
            daysRemaining > 0
        );
    }

    public async Task<List<OpenPortResult>> ScanLocalPortsAsync(string target, int startPort, int endPort, CancellationToken ct)
    {
        var openPorts = new List<OpenPortResult>();
        var tasks = new List<Task>();

        for (var port = startPort; port <= endPort; port++)
        {
            var currentPort = port;
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    using var client = new TcpClient();
                    var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    cts.CancelAfter(200);
                    await client.ConnectAsync(target, currentPort, cts.Token);
                    lock (openPorts)
                    {
                        openPorts.Add(new OpenPortResult(currentPort, "TCP", null));
                    }
                }
                catch
                {
                }
            }, ct));
        }

        await Task.WhenAll(tasks);
        return openPorts.OrderBy(p => p.Port).ToList();
    }

    public async Task<string> SpoofMacAsync(string interfaceName, CancellationToken ct)
    {
        var random = new Random();
        var mac = string.Join(":", Enumerable.Range(0, 6).Select(_ => random.Next(0, 256).ToString("x2")));
        var result = await scriptRunner.ExecuteAsync("sudo", $"ifconfig {interfaceName} ether {mac}", ".", ct);
        return $"MAC address changed to {mac} on {interfaceName}. {result}";
    }
}

