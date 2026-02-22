using System.Net.Http.Headers;
using System.Net.Http.Json;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.LottieFiles.Dtos;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Adapters;

public class LottieFilesAdapter(IHttpClientFactory httpClientFactory, ILogger<LottieFilesAdapter> logger) : ILottieFilesAdapter
{
    private const string BaseUrl = "https://api.lottiefiles.com/v2";

    public async Task<List<LottieAnimationDto>> SearchAnimationsAsync(string accessToken, string query, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        try
        {
            var response = await client.GetFromJsonAsync<LottieSearchResponse>($"{BaseUrl}/featured?page=1", ct);
            return response?.Data?.Select(a => new LottieAnimationDto(
                a.Id.ToString(), a.Name, a.PreviewUrl ?? "", a.LottieUrl ?? "", a.CreatedBy?.Name ?? "Unknown", a.LikesCount
            )).ToList() ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to search LottieFiles for {Query}", query);
            return [];
        }
    }

    public async Task<byte[]> DownloadAnimationAsync(string accessToken, string animationId, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        return await client.GetByteArrayAsync($"{BaseUrl}/animations/{animationId}/download", ct);
    }

    private HttpClient CreateClient(string accessToken)
    {
        var client = httpClientFactory.CreateClient("AtlasClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private record LottieSearchResponse(List<LottieAnimation>? Data);
    private record LottieAnimation(int Id, string Name, string? PreviewUrl, string? LottieUrl, LottieUser? CreatedBy, int LikesCount);
    private record LottieUser(string? Name);
}

