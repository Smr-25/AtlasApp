using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsUtilities.Queries.IpDnsLookup;

public class IpDnsLookupQueryHandler(
    ISecOpsUtilityService secOpsUtility
) : IRequestHandler<IpDnsLookupQuery, IpDnsLookupResult>
{
    public async Task<IpDnsLookupResult> Handle(IpDnsLookupQuery request, CancellationToken cancellationToken)
    {
        return await secOpsUtility.LookupIpDnsAsync(request.Target, cancellationToken);
    }
}

