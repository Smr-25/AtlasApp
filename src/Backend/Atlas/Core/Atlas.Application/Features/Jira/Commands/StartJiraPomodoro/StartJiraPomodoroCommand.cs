using MediatR;

namespace Atlas.Application.Features.Jira.Commands.StartJiraPomodoro;

public record StartJiraPomodoroCommand(
    Guid IntegrationId,
    string IssueKey,
    string DomainUrl,
    int DurationMinutes = 25,
    int BreakDurationMinutes = 5) : IRequest<Guid>;

