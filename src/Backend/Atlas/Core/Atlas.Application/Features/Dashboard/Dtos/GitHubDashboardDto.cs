using Atlas.Application.Features.GitHub.Dtos;

namespace Atlas.Application.Features.Dashboard.Dtos;

public record GitHubDashboardDto(
    List<GitHubWorkItemDto> MyPullRequests,
    List<GitHubWorkItemDto> ReviewRequested,
    List<GitHubWorkItemDto> MyIssues
);