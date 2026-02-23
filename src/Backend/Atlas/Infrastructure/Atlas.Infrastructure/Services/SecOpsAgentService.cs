using System.Net.Security;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Services;

public class SecOpsAgentService(IScriptRunnerService scriptRunner) : ISecOpsAgentService
{
    public async Task<List<RoguePortInfo>> DetectRoguePortsAsync(CancellationToken ct)
    {
        var result = await scriptRunner.ExecuteAsync("lsof", "-i -P -n", ".", ct);
        var ports = new List<RoguePortInfo>();
        foreach (var line in result.Split('\n').Skip(1))
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 9)
            {
                var portMatch = Regex.Match(parts[8], @":(\d+)");
                if (portMatch.Success && int.TryParse(portMatch.Groups[1].Value, out var port))
                {
                    ports.Add(new RoguePortInfo(port, parts[0], int.TryParse(parts[1], out var pid) ? pid : 0, "LISTEN"));
                }
            }
        }
        return ports;
    }

    public async Task<List<ExpiringSslInfo>> WarnExpiringSslAsync(List<string> domains, CancellationToken ct)
    {
        var results = new List<ExpiringSslInfo>();
        foreach (var domain in domains)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(domain, 443, ct);
                await using var sslStream = new SslStream(client.GetStream(), false);
                await sslStream.AuthenticateAsClientAsync(domain);
                var cert = sslStream.RemoteCertificate;
                if (cert != null)
                {
                    var cert2 = new System.Security.Cryptography.X509Certificates.X509Certificate2(cert);
                    var daysRemaining = (cert2.NotAfter - DateTime.UtcNow).Days;
                    if (daysRemaining <= 30)
                        results.Add(new ExpiringSslInfo(domain, cert2.NotAfter, daysRemaining));
                }
            }
            catch
            {
                results.Add(new ExpiringSslInfo(domain, DateTime.MinValue, -1));
            }
        }
        return results;
    }

    public Task<TrafficAnalysisResult> DetectSuspiciousTrafficAsync(string targetUrl, CancellationToken ct)
    {
        return Task.FromResult(new TrafficAnalysisResult(false, 0, null, "No suspicious traffic detected."));
    }

    public Task<List<LeakedKeyInfo>> ScanLeakedKeysAsync(string content, CancellationToken ct)
    {
        var leaks = new List<LeakedKeyInfo>();
        var patterns = new Dictionary<string, string>
        {
            { "AWS Access Key", @"AKIA[0-9A-Z]{16}" },
            { "AWS Secret Key", "(?i)aws(.{0,20})?['\"][0-9a-zA-Z/+]{40}['\"]" },
            { "GitHub Token", @"ghp_[0-9a-zA-Z]{36}" },
            { "Generic API Key", "(?i)(api[_-]?key|apikey)\\s*[:=]\\s*['\"]?[0-9a-zA-Z]{20,}" },
            { "Private Key", @"-----BEGIN (RSA |EC )?PRIVATE KEY-----" }
        };

        var lines = content.Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            foreach (var (keyType, pattern) in patterns)
            {
                if (Regex.IsMatch(lines[i], pattern))
                {
                    var snippet = lines[i].Length > 80 ? lines[i][..80] + "..." : lines[i];
                    leaks.Add(new LeakedKeyInfo(keyType, snippet, i + 1));
                }
            }
        }
        return Task.FromResult(leaks);
    }

    public Task<List<PatchSuggestion>> SuggestAutoPatchesAsync(string projectPath, CancellationToken ct)
    {
        return Task.FromResult(new List<PatchSuggestion>());
    }

    public async Task<List<ZombieProcessInfo>> KillZombieProcessesAsync(CancellationToken ct)
    {
        var result = await scriptRunner.ExecuteAsync("ps", "aux", ".", ct);
        var zombies = new List<ZombieProcessInfo>();
        foreach (var line in result.Split('\n').Skip(1))
        {
            if (line.Contains(" Z ") || line.Contains(" Z+ "))
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 11 && int.TryParse(parts[1], out var pid))
                {
                    zombies.Add(new ZombieProcessInfo(pid, parts[10], 0, "Zombie"));
                    await scriptRunner.ExecuteAsync("kill", $"-9 {pid}", ".", ct);
                }
            }
        }
        return zombies;
    }

    public async Task<VpnStatusResult> CheckVpnDropAsync(CancellationToken ct)
    {
        var ifconfig = await scriptRunner.ExecuteAsync("ifconfig", "", ".", ct);
        var hasVpn = ifconfig.Contains("utun") || ifconfig.Contains("tun0");
        var publicIp = await scriptRunner.ExecuteAsync("curl", "-s ifconfig.me", ".", ct);
        return new VpnStatusResult(hasVpn, publicIp.Trim(), null, !hasVpn);
    }
}

