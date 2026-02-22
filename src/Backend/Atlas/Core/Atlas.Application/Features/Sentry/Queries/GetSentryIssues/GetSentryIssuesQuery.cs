using Atlas.Application.Features.Sentry.Dtos;
using MediatR;

namespace Atlas.Application.Features.Sentry.Queries.GetSentryIssues;

public record GetSentryIssuesQuery(Guid IntegrationId, string ProjectSlug) : IRequest<List<SentryIssueDto>>;

