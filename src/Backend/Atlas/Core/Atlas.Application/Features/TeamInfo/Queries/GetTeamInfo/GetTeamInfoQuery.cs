using Atlas.Application.Features.TeamInfo.Dtos;
using MediatR;

namespace Atlas.Application.Features.TeamInfo.Queries.GetTeamInfo;

public record GetTeamInfoQuery(Guid TeamId) : IRequest<TeamInfoDto>;

