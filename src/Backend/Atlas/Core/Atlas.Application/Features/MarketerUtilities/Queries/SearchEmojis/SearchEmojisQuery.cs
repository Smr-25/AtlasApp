using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerUtilities.Queries.SearchEmojis;

public record SearchEmojisQuery(string Query) : IRequest<List<EmojiResult>>;

