using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.OmniFeed.Commands.PublishManualItem;

public class PublishManualItemCommandHandler(
    IOmniFeedService feedService,
    ICurrentUserService currentUser
) : IRequestHandler<PublishManualItemCommand, Unit>
{
    public async Task<Unit> Handle(PublishManualItemCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        await feedService.PublishManualItemAsync(request.TeamId, userId, request.Title, request.Body, cancellationToken);
        return Unit.Value;
    }
}

