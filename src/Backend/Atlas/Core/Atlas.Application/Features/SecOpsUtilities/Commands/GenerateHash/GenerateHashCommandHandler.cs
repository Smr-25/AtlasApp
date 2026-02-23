using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsUtilities.Commands.GenerateHash;

public class GenerateHashCommandHandler(
    ISecOpsUtilityService secOpsUtility
) : IRequestHandler<GenerateHashCommand, string>
{
    public Task<string> Handle(GenerateHashCommand request, CancellationToken cancellationToken)
    {
        var result = secOpsUtility.GenerateHash(request.Input, request.Algorithm);
        return Task.FromResult(result);
    }
}

