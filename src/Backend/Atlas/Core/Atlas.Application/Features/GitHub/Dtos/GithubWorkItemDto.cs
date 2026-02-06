namespace Atlas.Application.Features.GitHub.Dtos;

public record GitHubWorkItemDto(
    string Id,
    string Title,
    string RepoName,
    string Url,
    string Type,         
    string State,       
    string Author,
    DateTime UpdatedAt,
    bool IsDraft = false,
    int? CommentsCount = 0,
    string? BuildStatus = null 
);