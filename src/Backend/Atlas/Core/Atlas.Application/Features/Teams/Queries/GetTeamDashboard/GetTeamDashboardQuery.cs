using Atlas.Application.Features.Teams.Dtos;
using MediatR;

namespace Atlas.Application.Features.Teams.Queries.GetTeamDashboard;

public record GetTeamDashboardQuery(Guid TeamId) : IRequest<TeamDto>;

