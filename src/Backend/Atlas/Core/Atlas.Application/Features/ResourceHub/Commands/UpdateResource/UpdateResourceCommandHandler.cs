using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.ResourceHub.Commands.UpdateResource;

public class UpdateResourceCommandHandler(
    IApplicationDbContext dbContext
) : IRequestHandler<UpdateResourceCommand, Unit>
{
    public async Task<Unit> Handle(UpdateResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await dbContext.SharedResources.FirstOrDefaultAsync(r => r.Id == request.ResourceId, cancellationToken);
        if (resource != null)
        {
            resource.Update(request.Title, request.Url, request.Category, request.Description);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        return Unit.Value;
    }
}

