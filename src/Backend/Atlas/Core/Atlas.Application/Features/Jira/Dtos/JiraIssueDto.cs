namespace Atlas.Application.Features.Jira.Dtos;

public record JiraIssueDto(
    string Key,
    string Summary,
    string Type,
    string Status,
    string Assignee,
    string Priority,
    string Url
);