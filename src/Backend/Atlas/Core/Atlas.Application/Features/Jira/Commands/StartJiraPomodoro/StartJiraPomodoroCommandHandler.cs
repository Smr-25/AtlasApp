using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Jira.Commands.StartJiraPomodoro;

public class StartJiraPomodoroCommandHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    IJiraAdapter jiraAdapter,
    ICurrentUserService currentUserService
) : IRequestHandler<StartJiraPomodoroCommand, Guid>
{
    public async Task<Guid> Handle(StartJiraPomodoroCommand request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken);
        var issue = await jiraAdapter.GetIssueAsync(token, request.DomainUrl, request.IssueKey, cancellationToken);

        var session = FocusSession.Create(
            request.DurationMinutes,
            $"JIRA: {issue.Key} - {issue.Summary}",
            Guid.Parse(currentUserService.UserId!),
            FocusSessionType.Pomodoro,
            request.BreakDurationMinutes);

        dbContext.FocusSessions.Add(session);
        await dbContext.SaveChangesAsync(cancellationToken);

        return session.Id;
    }
}


