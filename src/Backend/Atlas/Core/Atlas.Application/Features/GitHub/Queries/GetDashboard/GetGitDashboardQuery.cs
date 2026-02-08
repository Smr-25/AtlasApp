using Atlas.Application.Features.GitHub.Dtos;
using MediatR;

namespace Atlas.Application.Features.GitHub.Queries.GetDashboard;

public record GitDashboardVm(
    List<GitWorkItemDto> MyWorkItems,
    List<GitWorkItemDto> ReviewRequests
);

public record GetGitDashboardQuery(Guid IntegrationId) : IRequest<GitDashboardVm>;