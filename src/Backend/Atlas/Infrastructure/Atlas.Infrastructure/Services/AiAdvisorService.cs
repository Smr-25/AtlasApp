using System.Net.Http.Json;
using System.Text.Json;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.System.Dtos;
using Atlas.Application.Settings;
using Microsoft.Extensions.Options;

namespace Atlas.Infrastructure.Services;

public class AiAdvisorService(IHttpClientFactory httpClientFactory,IOptions<AiSettings> options) : IAiAdvisorService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("OpenAI");
    private readonly string _apiKey = options.Value.ApiKey;

    public async Task<AiHealthAdviceDto> AnalyzeHealthAsync(SystemSnapshotDto snapshot)
    {
        if (string.IsNullOrEmpty(_apiKey))
            return new AiHealthAdviceDto("AI Key Missing", "Please configure API Key.", false, "Normal");

        var prompt = $@"
        You are a System Engineer AI. Analyze this computer status and give concise advice.
        
        DATA:
        - Battery: {snapshot.BatteryPercentage}% ({snapshot.BatteryStatus})
        - Remaining Time: {snapshot.RemainingMinutes} min
        - RAM: {snapshot.MemoryUsedGb}GB / {snapshot.TotalMemoryGb}GB
        - CPU Load: {snapshot.CpuLoad}%
        - Top Processes: {string.Join(", ", snapshot.TopProcesses.Select(p => $"{p.Name} ({p.MemoryMb}MB)"))}

        TASK:
        Return a JSON object with these fields:
        - summary: (string) Current state overview.
        - actionableAdvice: (string) Specific command or action to take (e.g., 'Kill Docker').
        - isCritical: (bool) True if battery < 20 or RAM > 90.
        - optimizedMode: (string) 'Performance', 'Balanced', or 'Survival'.
        
        Do NOT use markdown code blocks. Just raw JSON.";

        var requestBody = new
        {
            model = "gpt-5.2", 
            messages = new[]
            {
                new { role = "system", content = "You are a helpful system optimization assistant." },
                new { role = "user", content = prompt }
            },
            temperature = 0.7
        };
        
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
        
        if (!response.IsSuccessStatusCode)
            return new AiHealthAdviceDto("AI Error", "Could not connect to AI service.", false, "Normal");

        var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
        var contentText = jsonResponse.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        try 
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<AiHealthAdviceDto>(contentText!, options) 
                   ?? new AiHealthAdviceDto("Parse Error", "AI returned invalid format.", false, "Normal");
        }
        catch
        {
            return new AiHealthAdviceDto("Raw AI Response", contentText ?? "No response", false, "Normal");
        }
    }
}