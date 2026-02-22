using Atlas.Application.Features.Zeplin.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface IZeplinAdapter
{
    Task<List<ZeplinScreenDto>> GetScreensAsync(string accessToken, string projectId, CancellationToken ct);
    Task<ZeplinStyleGuideDto> GetStyleGuideAsync(string accessToken, string projectId, CancellationToken ct);
    Task NotifyDevelopersAsync(string accessToken, string projectId, string screenId, CancellationToken ct);
}

