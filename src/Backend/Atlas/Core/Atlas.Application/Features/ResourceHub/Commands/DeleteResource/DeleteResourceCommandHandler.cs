using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.ResourceHub.Commands.DeleteResource;

public class DeleteResourceCommandHandler(
    IApplicationDbContext dbContext
) : IRequestHandler<DeleteResourceCommand, Unit>
{
    public async Task<Unit> Handle(DeleteResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await dbContext.SharedResources.FirstOrDefaultAsync(r => r.Id == request.ResourceId, cancellationToken);
        if (resource != null)
        {
            dbContext.SharedResources.Remove(resource);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        return Unit.Value;
    }
}

