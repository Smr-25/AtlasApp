using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DevUtilities.Commands.GenerateSshKey;

public class GenerateSshKeyCommandHandler(
    IDevUtilityService devUtility
) : IRequestHandler<GenerateSshKeyCommand, SshKeyPairResult>
{
    public Task<SshKeyPairResult> Handle(GenerateSshKeyCommand request, CancellationToken cancellationToken)
    {
        var result = devUtility.GenerateSshKey(request.Comment, request.KeySize);
        return Task.FromResult(result);
    }
}

