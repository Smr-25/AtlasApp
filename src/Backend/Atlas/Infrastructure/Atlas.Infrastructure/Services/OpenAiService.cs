using System.Net.Http.Json;
using System.Text.Json;
using Atlas.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Atlas.Infrastructure.Services;

public class OpenAiService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    : IAiService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("OpenAI");
    private readonly string? _apiKey = configuration.GetSection("ThirdPartyServices:AiSettings:OpenAiApiKey").Value;
    private readonly string? _model = configuration.GetSection("ThirdPartyServices:AiSettings:Model").Value;

    public async Task<string> GenerateResponseAsync(string systemMessage, string userMessage, CancellationToken cancellationToken)
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
        
        var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return "Error: Unable to get response from OpenAI API.";
        
        var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        return result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
    }
}