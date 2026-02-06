namespace Atlas.Application.Features.GitHub.Dtos;

public record RepoStatsDto(
    string DefaultBranch,
    string Language,
    int Stars,
    int Forks,
    int OpenIssues,
    List<int> CommitActivity, 
    int LastMonthCommitCount
);