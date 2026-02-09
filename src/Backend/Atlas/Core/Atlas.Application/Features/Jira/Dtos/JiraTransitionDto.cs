namespace Atlas.Application.Features.Jira.Dtos;

public record JiraTransitionDto(
    string Id,
    string Name,
    string ToStatus
);

