using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Scripts.Commands.RunScript;

public class RunScriptCommandHandler(IApplicationDbContext applicationDbContext, IScriptRunnerService runner)
    : IRequestHandler<RunScriptCommand, string>
{
    public async Task<string> Handle(RunScriptCommand request, CancellationToken cancellationToken)
    {
        var script = await applicationDbContext.Scripts.FirstOrDefaultAsync(x => x.Id == request.Id,
            cancellationToken: cancellationToken);

        if (script == null) return "Script not found!";

        return await runner.ExecuteAsync(script.Command, script.Arguments, script.WorkingDirectory!, cancellationToken);
    }
}