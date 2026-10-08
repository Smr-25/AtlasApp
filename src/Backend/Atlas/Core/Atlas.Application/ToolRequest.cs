using System.Text.Json;
namespace Atlas.Application;

public record ToolRequest(string Tool, string Input = "", string Secondary = "", string[]? Paths = null,
    string OutputDirectory = "", Dictionary<string, string>? Options = null)
{
    public string Option(string key, string fallback = "") => Options?.GetValueOrDefault(key, fallback) ?? fallback;
    public int Number(string key, int fallback, int minimum, int maximum)
    {
        var value = Option(key);
        if (string.IsNullOrWhiteSpace(value)) return fallback;
        if (!int.TryParse(value, out var number) || number < minimum || number > maximum)
            throw new ArgumentException($"{key} must be between {minimum} and {maximum}.");
        return number;
    }
    public bool Flag(string key) => Option(key) == "true";
}
public record ToolResult(string Text, string[]? Files = null, string[][]? Rows = null);
public record ToolEvent(string Type, string? Text = null, double? Progress = null, string[]? Files = null, string[][]? Rows = null);
public static class ToolJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
}
