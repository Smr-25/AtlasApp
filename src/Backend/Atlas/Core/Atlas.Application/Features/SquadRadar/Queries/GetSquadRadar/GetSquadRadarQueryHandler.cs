using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SquadRadar.Queries.GetSquadRadar;

public class GetSquadRadarQueryHandler(
    ISquadRadarService radarService
) : IRequestHandler<GetSquadRadarQuery, SquadRadarSnapshot>
{
    public async Task<SquadRadarSnapshot> Handle(GetSquadRadarQuery request, CancellationToken cancellationToken)
    {
        return await radarService.GetRadarSnapshotAsync(request.TeamId, cancellationToken);
    }
}

