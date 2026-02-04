using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.CreateScript;

public class CreateScriptCommandHandler(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
    : IRequestHandler<CreateScriptCommand, Guid>
{
    public async Task<Guid> Handle(CreateScriptCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
        var script = Script.Create(
            request.Name,
            request.Command,
            request.Arguments,
            request.WorkingDirectory,
            request.Icon,
            request.Color,
            userId
        );
        
        await applicationDbContext.Scripts.AddAsync(script, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return script.Id;
    }
}