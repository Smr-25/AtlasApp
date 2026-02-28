using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.OmniFeed.Commands.AddEmojiReaction;

public class AddEmojiReactionCommandHandler(
    IOmniFeedService feedService,
    IAtlasHubService hubService,
    ICurrentUserService currentUser
) : IRequestHandler<AddEmojiReactionCommand, Unit>
{
    public async Task<Unit> Handle(AddEmojiReactionCommand request, CancellationToken cancellationToken)
    {
        await feedService.AddEmojiAsync(request.ItemId, request.Emoji, cancellationToken);

        var reactedBy = currentUser.GetUserIdOrDefault();
        var payload = new { request.ItemId, request.Emoji, ReactedBy = reactedBy };

        await hubService.SendFeedUpdateAsync(Guid.Empty, "ReactionAdded", payload, cancellationToken);

        return Unit.Value;
    }
}
