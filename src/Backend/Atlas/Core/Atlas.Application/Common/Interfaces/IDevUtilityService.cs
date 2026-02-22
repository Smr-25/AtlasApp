namespace Atlas.Application.Common.Interfaces;

public interface IDevUtilityService
{
    string DecodeJwt(string token);
    List<RegexMatchResult> TestRegex(string pattern, string input);
    string GenerateCron(string description);
    string EncodeBase64(string input);
    string DecodeBase64(string input);
    SshKeyPairResult GenerateSshKey(string comment, int keySize = 4096);
}

public record RegexMatchResult(string Value, int Index, int Length, Dictionary<string, string> Groups);
public record SshKeyPairResult(string PublicKey, string PrivateKey, string Fingerprint);

