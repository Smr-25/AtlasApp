using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.System.Dtos;
using MediatR;

namespace Atlas.Application.Features.System.Queries.GetActiveIdes;

public class GetActiveIdesQueryHandler(ISystemMonitorService systemService)
    : IRequestHandler<GetActiveIdesQuery, List<IdeStatusDto>>
{
    public async Task<List<IdeStatusDto>> Handle(GetActiveIdesQuery request, CancellationToken cancellationToken)
    {
        return await systemService.GetActiveIdesAsync(cancellationToken);
    }
}