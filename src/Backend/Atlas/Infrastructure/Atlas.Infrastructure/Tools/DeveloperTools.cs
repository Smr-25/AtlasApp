using System.Text.Json;
using System.Text;
using System.Text.RegularExpressions;
using Atlas.Application.Common.Interfaces;
namespace Atlas.Infrastructure.Tools;

public class DeveloperTools : IDeveloperTools
{
    public string DecodeJwt(string token)
    {
        var parts = token.Trim().Split('.');
        if (parts.Length != 3) throw new ArgumentException("A JWT must have three dot-separated parts. Decoding does not verify its signature.");
        var payload = parts[1].Replace('-', '+').Replace('_', '/');
        payload = payload.PadRight((payload.Length + 3) / 4 * 4, '=');
        return Encoding.UTF8.GetString(Convert.FromBase64String(payload));
    }
    public List<RegexMatchResult> TestRegex(string pattern, string input)
    {
        var regex = new Regex(pattern, RegexOptions.None, TimeSpan.FromSeconds(1));
        var matches = regex.Matches(input).Take(10001).ToArray();
        if (matches.Length > 10000) throw new ArgumentException("More than 10,000 matches. Narrow the pattern or input.");
        return matches.Select(m => new RegexMatchResult(m.Value, m.Index, m.Length,
            m.Groups.Cast<Group>().Skip(1).Where(g => g.Success).ToDictionary(g => g.Name, g => g.Value))).ToList();
    }
    public string EncodeBase64(string input) => Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
    public string DecodeBase64(string input) => new UTF8Encoding(false, true).GetString(Convert.FromBase64String(input));

    public string FormatJson(string json) => Format(json, true);
    public string MinifyJson(string json) => Format(json, false);
    private static string Format(string json, bool indent)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("Paste JSON to get started.");
        using var document = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(document.RootElement, new JsonSerializerOptions { WriteIndented = indent });
    }
    public string DiffJson(string json1, string json2)
    {
        using var left = JsonDocument.Parse(json1);
        using var right = JsonDocument.Parse(json2);
        return JsonElement.DeepEquals(left.RootElement, right.RootElement) ? "JSONs are identical." : "JSONs are different";
    }

}
