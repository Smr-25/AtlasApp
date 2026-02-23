using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsUtilities.Queries.CalculatePasswordEntropy;

public class CalculatePasswordEntropyQueryHandler(
    ISecOpsUtilityService secOpsUtility
) : IRequestHandler<CalculatePasswordEntropyQuery, PasswordEntropyResult>
{
    public Task<PasswordEntropyResult> Handle(CalculatePasswordEntropyQuery request, CancellationToken cancellationToken)
    {
        var result = secOpsUtility.CalculateEntropy(request.Password);
        return Task.FromResult(result);
    }
}

