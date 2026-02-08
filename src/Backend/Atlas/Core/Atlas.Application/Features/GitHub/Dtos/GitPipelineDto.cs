namespace Atlas.Application.Features.GitHub.Dtos;

public record GitPipelineDto(
    string Id,
    string Status,
    string BranchName,
    string CommitMessage,
    string Url,
    DateTime StartedAt,
    TimeSpan? Duration
);