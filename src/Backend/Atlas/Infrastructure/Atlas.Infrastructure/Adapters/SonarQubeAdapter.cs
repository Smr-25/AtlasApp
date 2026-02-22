using System.Net.Http.Headers;
using System.Net.Http.Json;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.SonarQube.Dtos;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Adapters;

public class SonarQubeAdapter(IHttpClientFactory httpClientFactory, ILogger<SonarQubeAdapter> logger) : ISonarQubeAdapter
{
    public async Task<SonarQubeProjectQualityDto> GetProjectQualityAsync(string accessToken, string projectKey, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        try
        {
            var url = $"api/measures/component?component={projectKey}&metricKeys=bugs,vulnerabilities,code_smells,coverage,duplicated_lines_density,reliability_rating,security_rating,sqale_rating";
            var response = await client.GetFromJsonAsync<SonarQubeMetricsResponse>(url, ct);
            if (response?.Component?.Measures == null)
                return new SonarQubeProjectQualityDto(projectKey, projectKey, "A", "A", "A", 0, 0, 0, 0, 0);

            return MapToDto(projectKey, response.Component.Measures);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch SonarQube quality for {Project}", projectKey);
            return new SonarQubeProjectQualityDto(projectKey, projectKey, "N/A", "N/A", "N/A", 0, 0, 0, 0, 0);
        }
    }

    public async Task<List<SonarQubeIssueDto>> GetIssuesAsync(string accessToken, string projectKey, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        try
        {
            var url = $"api/issues/search?componentKeys={projectKey}&resolved=false&ps=50";
            var response = await client.GetFromJsonAsync<SonarQubeIssuesResponse>(url, ct);
            return response?.Issues?.Select(i => new SonarQubeIssueDto(
                i.Key, i.Message, i.Severity, i.Component, i.Line, i.Type, i.Status
            )).ToList() ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch SonarQube issues for {Project}", projectKey);
            return [];
        }
    }

    private static SonarQubeProjectQualityDto MapToDto(string projectKey, List<SonarMeasure> measures)
    {
        string GetValue(string key) => measures.FirstOrDefault(m => m.Metric == key)?.Value ?? "0";
        string RatingToGrade(string val) => val switch { "1" or "1.0" => "A", "2" or "2.0" => "B", "3" or "3.0" => "C", "4" or "4.0" => "D", _ => "E" };

        return new SonarQubeProjectQualityDto(
            projectKey, projectKey,
            RatingToGrade(GetValue("reliability_rating")),
            RatingToGrade(GetValue("security_rating")),
            RatingToGrade(GetValue("sqale_rating")),
            double.TryParse(GetValue("coverage"), out var cov) ? cov : 0,
            int.TryParse(GetValue("bugs"), out var b) ? b : 0,
            int.TryParse(GetValue("vulnerabilities"), out var v) ? v : 0,
            int.TryParse(GetValue("code_smells"), out var cs) ? cs : 0,
            int.TryParse(GetValue("duplicated_lines_density"), out var d) ? d : 0);
    }

    private HttpClient CreateClient(string accessToken)
    {
        var client = httpClientFactory.CreateClient("AtlasClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private record SonarQubeMetricsResponse(SonarComponent? Component);
    private record SonarComponent(List<SonarMeasure> Measures);
    private record SonarMeasure(string Metric, string Value);
    private record SonarQubeIssuesResponse(List<SonarIssue>? Issues);
    private record SonarIssue(string Key, string Message, string Severity, string Component, int Line, string Type, string Status);
}

