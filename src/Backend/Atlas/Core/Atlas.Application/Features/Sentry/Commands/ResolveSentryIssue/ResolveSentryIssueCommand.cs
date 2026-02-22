using MediatR;

namespace Atlas.Application.Features.Sentry.Commands.ResolveSentryIssue;

public record ResolveSentryIssueCommand(Guid IntegrationId, string IssueId) : IRequest;

