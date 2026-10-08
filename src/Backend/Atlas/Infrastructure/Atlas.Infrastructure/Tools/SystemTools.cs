using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Atlas.Application.Common.Interfaces;
namespace Atlas.Infrastructure.Tools;

public class SystemTools : ISystemTools
{
    public string GenerateHash(string input, string algorithm)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = algorithm.ToUpperInvariant() switch
        {
            "SHA256" => SHA256.HashData(bytes), "SHA512" => SHA512.HashData(bytes),
            "SHA1" => SHA1.HashData(bytes), "MD5" => MD5.HashData(bytes),
            _ => throw new ArgumentException("Choose SHA256, SHA512, SHA1 or MD5.")
        };
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
    public async Task<string[]> LookupIpDnsAsync(string target, CancellationToken ct) =>
        (await Dns.GetHostAddressesAsync(target.Trim(), ct)).Select(x => x.ToString()).ToArray();
    public async Task<SslCheckResult> CheckSslAsync(string hostname, CancellationToken ct)
    {
        hostname = hostname.Trim();
        using var client = new TcpClient();
        await client.ConnectAsync(hostname, 443, ct);
        await using var stream = new SslStream(client.GetStream(), false);
        await stream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions { TargetHost = hostname }, ct);
        using var cert = new X509Certificate2(stream.RemoteCertificate ?? throw new IOException("No certificate returned."));
        return new(cert.Subject, cert.Issuer, cert.NotBefore, cert.NotAfter, (cert.NotAfter.ToUniversalTime() - DateTime.UtcNow).Days, true);
    }
}
