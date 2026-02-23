using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;

namespace Atlas.Application.Features.ResourceHub.Commands.AddResource;

public class AddResourceCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser
) : IRequestHandler<AddResourceCommand, Guid>
{
    public async Task<Guid> Handle(AddResourceCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var resource = SharedResource.Create(request.TeamId, userId, request.Title, request.Url, request.Category, request.Description);
        dbContext.SharedResources.Add(resource);
        await dbContext.SaveChangesAsync(cancellationToken);
        return resource.Id;
    }
}

