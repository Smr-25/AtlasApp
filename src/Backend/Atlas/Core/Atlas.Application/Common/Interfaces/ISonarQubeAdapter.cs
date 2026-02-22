using Atlas.Application.Features.SonarQube.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface ISonarQubeAdapter
{
    Task<SonarQubeProjectQualityDto> GetProjectQualityAsync(string accessToken, string projectKey, CancellationToken ct);
    Task<List<SonarQubeIssueDto>> GetIssuesAsync(string accessToken, string projectKey, CancellationToken ct);
}

