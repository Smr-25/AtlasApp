using Atlas.Application.Common.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; 

namespace Atlas.Infrastructure.Services;

public class JsonToolService : IJsonToolService
{
    public string FormatJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return string.Empty;

        try
        {
            var parsedJson = JToken.Parse(json);
            return parsedJson.ToString(Formatting.Indented);
        }
        catch (JsonReaderException ex)
        {
            throw new Exception($"Invalid JSON format: {ex.Message}");
        }
    }

    public string MinifyJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return string.Empty;

        try
        {
            var parsedJson = JToken.Parse(json);
            return parsedJson.ToString(Formatting.None);
        }
        catch (JsonReaderException ex)
        {
            throw new Exception($"Invalid JSON format: {ex.Message}");
        }
    }

    public string DiffJson(string json1, string json2)
    {
        
        try 
        {
            var j1 = JToken.Parse(json1);
            var j2 = JToken.Parse(json2);

                return JToken.DeepEquals(j1, j2) ? "JSONs are identical." : "JSONs are different";
        }
        catch (Exception ex)
        {
             throw new Exception($"Error comparing JSONs: {ex.Message}");
        }
    }
}