namespace Atlas.Application.Common.Interfaces;

public interface IDeveloperTools
{
    string FormatJson(string json);
    string MinifyJson(string json);
    string DiffJson(string left, string right);
    string DecodeJwt(string token);
    List<RegexMatchResult> TestRegex(string pattern, string input);
    string EncodeBase64(string input);
    string DecodeBase64(string input);
}

public record RegexMatchResult(string Value, int Index, int Length, Dictionary<string, string> Groups);

