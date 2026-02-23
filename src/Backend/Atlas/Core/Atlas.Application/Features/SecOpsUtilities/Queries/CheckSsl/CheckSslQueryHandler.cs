using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsUtilities.Queries.CheckSsl;

public class CheckSslQueryHandler(
    ISecOpsUtilityService secOpsUtility
) : IRequestHandler<CheckSslQuery, SslCheckResult>
{
    public async Task<SslCheckResult> Handle(CheckSslQuery request, CancellationToken cancellationToken)
    {
        return await secOpsUtility.CheckSslAsync(request.Hostname, cancellationToken);
    }
}

