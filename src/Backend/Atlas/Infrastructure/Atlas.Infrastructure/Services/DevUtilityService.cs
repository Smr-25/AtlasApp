using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Services;

public class DevUtilityService : IDevUtilityService
{
    public string DecodeJwt(string token)
    {
        var parts = token.Split('.');
        if (parts.Length < 2) return "{}";
        var payload = parts[1];
        var padded = payload.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }
        return Encoding.UTF8.GetString(Convert.FromBase64String(padded));
    }

    public List<RegexMatchResult> TestRegex(string pattern, string input)
    {
        var regex = new Regex(pattern);
        var matches = regex.Matches(input);
        return matches.Select(m => new RegexMatchResult(
            m.Value, m.Index, m.Length,
            m.Groups.Cast<Group>().Skip(1)
                .Where(g => g.Success)
                .ToDictionary(g => g.Name, g => g.Value)
        )).ToList();
    }

    public string GenerateCron(string description)
    {
        var lower = description.ToLowerInvariant();
        if (lower.Contains("every minute")) return "* * * * *";
        if (lower.Contains("every 5 min")) return "*/5 * * * *";
        if (lower.Contains("every 10 min")) return "*/10 * * * *";
        if (lower.Contains("every 15 min")) return "*/15 * * * *";
        if (lower.Contains("every 30 min")) return "*/30 * * * *";
        if (lower.Contains("every hour")) return "0 * * * *";
        if (lower.Contains("every day") || lower.Contains("daily")) return "0 0 * * *";
        if (lower.Contains("every week") || lower.Contains("weekly")) return "0 0 * * 0";
        if (lower.Contains("every month") || lower.Contains("monthly")) return "0 0 1 * *";
        if (lower.Contains("midnight")) return "0 0 * * *";
        if (lower.Contains("noon")) return "0 12 * * *";
        return "0 * * * *";
    }

    public string EncodeBase64(string input) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(input));

    public string DecodeBase64(string input) =>
        Encoding.UTF8.GetString(Convert.FromBase64String(input));

    public SshKeyPairResult GenerateSshKey(string comment, int keySize = 4096)
    {
        using var rsa = RSA.Create(keySize);
        var publicKeyBytes = rsa.ExportRSAPublicKey();
        var privateKeyBytes = rsa.ExportRSAPrivateKey();

        var publicKey = $"ssh-rsa {Convert.ToBase64String(publicKeyBytes)} {comment}";
        var privateKey = $"-----BEGIN RSA PRIVATE KEY-----\n{Convert.ToBase64String(privateKeyBytes, Base64FormattingOptions.InsertLineBreaks)}\n-----END RSA PRIVATE KEY-----";

        using var sha256 = SHA256.Create();
        var fingerprint = sha256.ComputeHash(publicKeyBytes);
        var fingerprintStr = $"SHA256:{Convert.ToBase64String(fingerprint).TrimEnd('=')}";

        return new SshKeyPairResult(publicKey, privateKey, fingerprintStr);
    }
}

