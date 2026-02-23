using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.OmniFeed.Commands.AddEmojiReaction;

public class AddEmojiReactionCommandHandler(
    IOmniFeedService feedService
) : IRequestHandler<AddEmojiReactionCommand, Unit>
{
    public async Task<Unit> Handle(AddEmojiReactionCommand request, CancellationToken cancellationToken)
    {
        await feedService.AddEmojiAsync(request.ItemId, request.Emoji, cancellationToken);
        return Unit.Value;
    }
}

