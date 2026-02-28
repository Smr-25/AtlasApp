using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.OmniFeed.Commands.PublishManualItem;

public class PublishManualItemCommandHandler(
    IOmniFeedService feedService,
    ICurrentUserService currentUser,
    IAtlasHubService hubService
) : IRequestHandler<PublishManualItemCommand, Unit>
{
    public async Task<Unit> Handle(PublishManualItemCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        await feedService.PublishManualItemAsync(request.TeamId, userId, request.Title, request.Body, cancellationToken);

        var payload = new {
            TeamId = request.TeamId,
            Title = request.Title,
            Body = request.Body,
            PublishedBy = userId
        };

        await hubService.SendFeedUpdateAsync(request.TeamId, "ManualItemPublished", payload, cancellationToken);

        return Unit.Value;
    }
}
