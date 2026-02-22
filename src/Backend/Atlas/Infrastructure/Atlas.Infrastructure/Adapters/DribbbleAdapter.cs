using System.Net.Http.Headers;
using System.Net.Http.Json;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Dribbble.Dtos;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Adapters;

public class DribbbleAdapter(IHttpClientFactory httpClientFactory, ILogger<DribbbleAdapter> logger) : IDribbbleAdapter
{
    private const string BaseUrl = "https://api.dribbble.com/v2";

    public async Task<List<DribbbleShotDto>> GetShotsAsync(string accessToken, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        try
        {
            var shots = await client.GetFromJsonAsync<List<DribbbleApiShot>>($"{BaseUrl}/user/shots", ct) ?? [];
            return shots.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch Dribbble shots");
            return [];
        }
    }

    public async Task<List<DribbbleShotDto>> SearchInspirationAsync(string accessToken, string query, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        try
        {
            var shots = await client.GetFromJsonAsync<List<DribbbleApiShot>>($"{BaseUrl}/shots?query={Uri.EscapeDataString(query)}", ct) ?? [];
            return shots.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to search Dribbble for {Query}", query);
            return [];
        }
    }

    private static DribbbleShotDto MapToDto(DribbbleApiShot s) =>
        new(s.Id.ToString(), s.Title, s.HtmlUrl ?? "", s.Images?.Normal ?? "",
            s.User?.Name ?? "Unknown", s.User?.AvatarUrl ?? "",
            s.LikesCount, s.ViewsCount, s.PublishedAt);

    private HttpClient CreateClient(string accessToken)
    {
        var client = httpClientFactory.CreateClient("AtlasClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private record DribbbleApiShot(int Id, string Title, string? HtmlUrl, DribbbleImages? Images, DribbbleUser? User, int LikesCount, int ViewsCount, DateTime PublishedAt);
    private record DribbbleImages(string? Normal);
    private record DribbbleUser(string? Name, string? AvatarUrl);
}

