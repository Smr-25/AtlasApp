using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsUtilities.Commands.SpoofMac;

public class SpoofMacCommandHandler(
    ISecOpsUtilityService secOpsUtility
) : IRequestHandler<SpoofMacCommand, string>
{
    public async Task<string> Handle(SpoofMacCommand request, CancellationToken cancellationToken)
    {
        return await secOpsUtility.SpoofMacAsync(request.InterfaceName, cancellationToken);
    }
}

