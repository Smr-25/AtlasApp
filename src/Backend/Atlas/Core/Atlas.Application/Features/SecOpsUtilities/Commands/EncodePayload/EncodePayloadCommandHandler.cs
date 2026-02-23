using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsUtilities.Commands.EncodePayload;

public class EncodePayloadCommandHandler(
    ISecOpsUtilityService secOpsUtility
) : IRequestHandler<EncodePayloadCommand, string>
{
    public Task<string> Handle(EncodePayloadCommand request, CancellationToken cancellationToken)
    {
        var result = secOpsUtility.EncodePayload(request.Input, request.Encoding);
        return Task.FromResult(result);
    }
}

