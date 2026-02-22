using System.Net.Http.Headers;
using System.Net.Http.Json;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Miro.Dtos;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Adapters;

public class MiroAdapter(IHttpClientFactory httpClientFactory, ILogger<MiroAdapter> logger) : IMiroAdapter
{
    private const string BaseUrl = "https://api.miro.com/v2";

    public async Task<List<MiroBoardDto>> GetBoardsAsync(string accessToken, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        try
        {
            var response = await client.GetFromJsonAsync<MiroBoardsResponse>($"{BaseUrl}/boards", ct);
            return response?.Data?.Select(b => new MiroBoardDto(
                b.Id, b.Name, b.Description ?? "", b.ViewLink ?? "", b.ModifiedAt, 0
            )).ToList() ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch Miro boards");
            return [];
        }
    }

    public async Task CreateStickyNoteAsync(string accessToken, string boardId, string content, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        var payload = new { data = new { content, shape = "square" }, type = "sticky_note" };
        var response = await client.PostAsJsonAsync($"{BaseUrl}/boards/{boardId}/sticky_notes", payload, ct);
        response.EnsureSuccessStatusCode();
    }

    private HttpClient CreateClient(string accessToken)
    {
        var client = httpClientFactory.CreateClient("AtlasClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private record MiroBoardsResponse(List<MiroBoard>? Data);
    private record MiroBoard(string Id, string Name, string? Description, string? ViewLink, DateTime ModifiedAt);
}

