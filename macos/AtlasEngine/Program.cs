using System.Text.Json;

var protocolOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

if (args.Contains("--self-test"))
{
    if (Transform("{\"a\":1}", "format") != "{\n  \"a\": 1\n}" ||
        Transform(" { \"a\" : 1 } ", "minify") != "{\"a\":1}")
        throw new Exception("JSON transform failed.");

    Console.WriteLine("AtlasEngine self-test passed.");
    return;
}

try
{
    var request = JsonSerializer.Deserialize<Request>(Console.In.ReadToEnd(), protocolOptions)
        ?? throw new ArgumentException("Empty request.");
    Console.WriteLine(JsonSerializer.Serialize(new Response(Transform(request.Input, request.Operation), null), protocolOptions));
}
catch (Exception ex) when (ex is JsonException or ArgumentException)
{
    Console.WriteLine(JsonSerializer.Serialize(new Response(null, ex.Message), protocolOptions));
}

static string Transform(string input, string operation)
{
    if (string.IsNullOrWhiteSpace(input)) throw new ArgumentException("Paste JSON to get started.");
    if (operation is not ("format" or "minify")) throw new ArgumentException("Unknown operation.");

    using var document = JsonDocument.Parse(input);
    return JsonSerializer.Serialize(document.RootElement, new JsonSerializerOptions
    {
        WriteIndented = operation == "format"
    });
}

record Request(string Input, string Operation);
record Response(string? Result, string? Error);
