using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SystemTools.Commands.KillProcess;

public class KillProcessCommandHandler(ISystemToolAdapter systemTool) 
    : IRequestHandler<KillProcessCommand, bool>
{
    public async Task<bool> Handle(KillProcessCommand request, CancellationToken cancellationToken)
    {
        await systemTool.KillProcessAsync(request.Pid, cancellationToken);
        return true;
    }
}