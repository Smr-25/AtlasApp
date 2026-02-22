using Atlas.Application.Features.Sentry.Dtos;
using MediatR;

namespace Atlas.Application.Features.Sentry.Queries.GetSentryIssueDetail;

public record GetSentryIssueDetailQuery(Guid IntegrationId, string IssueId) : IRequest<SentryIssueDetailDto>;

