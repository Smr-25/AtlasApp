using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.CreateScript;

public class CreateScriptCommandHandler(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
    : IRequestHandler<CreateScriptCommand, Guid>
{
    public async Task<Guid> Handle(CreateScriptCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
            throw new UnauthorizedAccessException("User is not authenticated or user ID is invalid.");
        
        
        var script = Script.Create(
            request.Name,
            request.Command,
            request.Arguments,
            request.WorkingDirectory,
            request.Icon,
            request.Color,
            parsedUserId
        );
        await applicationDbContext.Scripts.AddAsync(script, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return script.Id;
    }
}