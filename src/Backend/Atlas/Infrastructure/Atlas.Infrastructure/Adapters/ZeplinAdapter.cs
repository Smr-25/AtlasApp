using System.Net.Http.Headers;
using System.Net.Http.Json;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Zeplin.Dtos;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Adapters;

public class ZeplinAdapter(IHttpClientFactory httpClientFactory, ILogger<ZeplinAdapter> logger) : IZeplinAdapter
{
    private const string BaseUrl = "https://api.zeplin.dev/v1";

    public async Task<List<ZeplinScreenDto>> GetScreensAsync(string accessToken, string projectId, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        try
        {
            var screens = await client.GetFromJsonAsync<List<ZeplinApiScreen>>($"{BaseUrl}/projects/{projectId}/screens", ct) ?? [];
            return screens.Select(s => new ZeplinScreenDto(
                s.Id, s.Name, s.Image?.OriginalUrl ?? "", s.Width, s.Height, s.Updated
            )).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch Zeplin screens for {Project}", projectId);
            return [];
        }
    }

    public async Task<ZeplinStyleGuideDto> GetStyleGuideAsync(string accessToken, string projectId, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        try
        {
            var colors = await client.GetFromJsonAsync<List<ZeplinApiColor>>($"{BaseUrl}/projects/{projectId}/colors", ct) ?? [];
            var fonts = await client.GetFromJsonAsync<List<ZeplinApiFont>>($"{BaseUrl}/projects/{projectId}/text_styles", ct) ?? [];

            return new ZeplinStyleGuideDto(
                projectId,
                colors.Select(c => new ZeplinColorDto(c.Name, $"#{c.R:X2}{c.G:X2}{c.B:X2}", c.A)).ToList(),
                fonts.Select(f => new ZeplinFontDto(f.FontFamily, f.FontSize, f.FontWeight)).ToList(),
                []);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch Zeplin style guide for {Project}", projectId);
            return new ZeplinStyleGuideDto(projectId, [], [], []);
        }
    }

    public async Task NotifyDevelopersAsync(string accessToken, string projectId, string screenId, CancellationToken ct)
    {
        logger.LogInformation("Notifying developers about screen {ScreenId} in project {ProjectId}", screenId, projectId);
        await Task.CompletedTask;
    }

    private HttpClient CreateClient(string accessToken)
    {
        var client = httpClientFactory.CreateClient("AtlasClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private record ZeplinApiScreen(string Id, string Name, ZeplinImage? Image, int Width, int Height, DateTime Updated);
    private record ZeplinImage(string? OriginalUrl);
    private record ZeplinApiColor(string Name, int R, int G, int B, double A);
    private record ZeplinApiFont(string FontFamily, double FontSize, string FontWeight);
}

