using Atlas.Application.Features.Teams.Dtos;
using MediatR;

namespace Atlas.Application.Features.Teams.Queries.GetTeamProductivityReport;

public record GetTeamProductivityReportQuery(Guid TeamId) : IRequest<TeamProductivityReportDto>;

