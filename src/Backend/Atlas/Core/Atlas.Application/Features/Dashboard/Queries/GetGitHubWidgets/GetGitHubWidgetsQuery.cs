using Atlas.Application.Features.Dashboard.Dtos;
using MediatR;

namespace Atlas.Application.Features.Dashboard.Queries.GetGitHubWidgets;

public record GetGitHubWidgetsQuery(Guid IntegrationId) : IRequest<GitHubDashboardDto>;