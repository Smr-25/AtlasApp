using MediatR;

namespace Atlas.Application.Features.OmniFeed.Commands.MarkFeedItemRead;

public record MarkFeedItemReadCommand(Guid ItemId) : IRequest<Unit>;

