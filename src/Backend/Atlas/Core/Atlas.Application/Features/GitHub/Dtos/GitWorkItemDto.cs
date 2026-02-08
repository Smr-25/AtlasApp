namespace Atlas.Application.Features.GitHub.Dtos;

public record GitWorkItemDto(
    string Id,
    string Title,
    string RepositoryName,
    string Url,
    WorkItemType Type,
    WorkItemState State,
    string AuthorName,
    string AuthorAvatarUrl,
    DateTime UpdatedAt,
    bool? IsDraft,
    string? SourceBranch,
    string? TargetBranch,
    int CommentCount,
    string? CiStatus
);

public enum WorkItemType
{
    PullRequest,
    Issue
}

public enum WorkItemState
{
    Open,
    Merged,
    Closed,
    Draft
}