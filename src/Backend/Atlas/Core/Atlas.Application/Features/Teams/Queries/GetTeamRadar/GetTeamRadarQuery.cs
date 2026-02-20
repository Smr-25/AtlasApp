using Atlas.Application.Features.Teams.Dtos;
using MediatR;

namespace Atlas.Application.Features.Teams.Queries.GetTeamRadar;

public record GetTeamRadarQuery(Guid TeamId) : IRequest<TeamRadarDto>;

