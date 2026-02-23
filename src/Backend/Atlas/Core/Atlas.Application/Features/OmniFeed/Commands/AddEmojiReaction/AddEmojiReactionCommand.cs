using MediatR;

namespace Atlas.Application.Features.OmniFeed.Commands.AddEmojiReaction;

public record AddEmojiReactionCommand(Guid ItemId, string Emoji) : IRequest<Unit>;

