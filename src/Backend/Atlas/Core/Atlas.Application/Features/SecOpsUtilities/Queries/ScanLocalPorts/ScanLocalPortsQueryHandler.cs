using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsUtilities.Queries.ScanLocalPorts;

public class ScanLocalPortsQueryHandler(
    ISecOpsUtilityService secOpsUtility
) : IRequestHandler<ScanLocalPortsQuery, List<OpenPortResult>>
{
    public async Task<List<OpenPortResult>> Handle(ScanLocalPortsQuery request, CancellationToken cancellationToken)
    {
        return await secOpsUtility.ScanLocalPortsAsync(request.Target, request.StartPort, request.EndPort, cancellationToken);
    }
}

