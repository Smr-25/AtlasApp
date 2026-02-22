using System.Net.Http.Headers;
using System.Net.Http.Json;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Figma.Dtos;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Adapters;

public class FigmaAdapter(IHttpClientFactory httpClientFactory, ILogger<FigmaAdapter> logger) : IFigmaAdapter
{
    private const string BaseUrl = "https://api.figma.com/v1";

    public async Task<List<FigmaCommentDto>> GetCommentsAsync(string accessToken, string fileKey, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        try
        {
            var response = await client.GetFromJsonAsync<FigmaCommentsResponse>($"{BaseUrl}/files/{fileKey}/comments", ct);
            return response?.Comments?.Select(c => new FigmaCommentDto(
                c.Id, c.Message, c.User?.Handle ?? "Unknown", c.User?.ImgUrl ?? "",
                c.CreatedAt, c.ResolvedAt != null, c.ParentId
            )).ToList() ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch Figma comments for {FileKey}", fileKey);
            return [];
        }
    }

    public async Task PostCommentAsync(string accessToken, string fileKey, string message, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        var response = await client.PostAsJsonAsync($"{BaseUrl}/files/{fileKey}/comments", new { message }, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task ResolveCommentAsync(string accessToken, string fileKey, string commentId, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        var url = $"{BaseUrl}/files/{fileKey}/comments";
        var response = await client.PostAsJsonAsync(url, new { comment_id = commentId, message = "Resolved" }, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<FigmaComponentDto>> GetFileComponentsAsync(string accessToken, string fileKey, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        try
        {
            var response = await client.GetFromJsonAsync<FigmaFileResponse>($"{BaseUrl}/files/{fileKey}/components", ct);
            return response?.Meta?.Components?.Select(c => new FigmaComponentDto(
                c.Key, c.Name, c.Description ?? "", c.ThumbnailUrl ?? "", c.ContainingFrame?.Name ?? ""
            )).ToList() ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch Figma components for {FileKey}", fileKey);
            return [];
        }
    }

    private HttpClient CreateClient(string accessToken)
    {
        var client = httpClientFactory.CreateClient("AtlasClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private record FigmaCommentsResponse(List<FigmaApiComment>? Comments);
    private record FigmaApiComment(string Id, string Message, FigmaUser? User, DateTime CreatedAt, DateTime? ResolvedAt, string? ParentId);
    private record FigmaUser(string Handle, string ImgUrl);
    private record FigmaFileResponse(FigmaMeta? Meta);
    private record FigmaMeta(List<FigmaApiComponent>? Components);
    private record FigmaApiComponent(string Key, string Name, string? Description, string? ThumbnailUrl, FigmaFrame? ContainingFrame);
    private record FigmaFrame(string? Name);
}

