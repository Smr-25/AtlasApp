using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SquadRadar.Queries.GetSquadRadar;

public record GetSquadRadarQuery(Guid TeamId) : IRequest<SquadRadarSnapshot>;

