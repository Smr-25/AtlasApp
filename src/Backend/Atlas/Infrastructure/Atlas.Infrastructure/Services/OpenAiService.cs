using System.Net.Http.Json;
using System.Text.Json;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Atlas.Infrastructure.Services;

public class OpenAiService(IOptions<AiSettings> settings, IHttpClientFactory httpClientFactory)
    : IAiService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("OpenAI");
    private readonly string _model = settings.Value.Model;

    public async Task<string> GenerateResponseAsync(string systemMessage, string userMessage,
        CancellationToken cancellationToken)
    {
        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = systemMessage },
                new { role = "user", content = userMessage }
            },
            temperature = 0.7
        };

        var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            return "Error: Unable to get response from OpenAI API.";

        var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        return result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
    }
}