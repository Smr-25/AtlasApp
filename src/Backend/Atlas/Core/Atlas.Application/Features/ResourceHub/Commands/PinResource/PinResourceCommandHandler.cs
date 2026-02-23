using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.ResourceHub.Commands.PinResource;

public class PinResourceCommandHandler(
    IApplicationDbContext dbContext
) : IRequestHandler<PinResourceCommand, Unit>
{
    public async Task<Unit> Handle(PinResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await dbContext.SharedResources.FirstOrDefaultAsync(r => r.Id == request.ResourceId, cancellationToken);
        if (resource != null)
        {
            resource.TogglePin();
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        return Unit.Value;
    }
}

