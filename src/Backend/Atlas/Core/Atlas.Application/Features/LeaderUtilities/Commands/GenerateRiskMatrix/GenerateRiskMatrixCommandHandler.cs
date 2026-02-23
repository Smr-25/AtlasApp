using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderUtilities.Commands.GenerateRiskMatrix;

public class GenerateRiskMatrixCommandHandler(
    ILeaderUtilityService utilityService
) : IRequestHandler<GenerateRiskMatrixCommand, RiskMatrixResult>
{
    public Task<RiskMatrixResult> Handle(GenerateRiskMatrixCommand request, CancellationToken cancellationToken)
    {
        var result = utilityService.GenerateRiskMatrix(request.Items);
        return Task.FromResult(result);
    }
}

