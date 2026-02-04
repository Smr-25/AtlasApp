using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
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

        if (script == null)
            throw new NotFoundException(nameof(Script), request.Id);

        return await runner.ExecuteAsync(script.Command, script.Arguments, script.WorkingDirectory ?? string.Empty, cancellationToken);
    }
}