using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.OmniFeed.Commands.MarkFeedItemRead;

public class MarkFeedItemReadCommandHandler(
    IOmniFeedService feedService
) : IRequestHandler<MarkFeedItemReadCommand, Unit>
{
    public async Task<Unit> Handle(MarkFeedItemReadCommand request, CancellationToken cancellationToken)
    {
        await feedService.MarkAsReadAsync(request.ItemId, cancellationToken);
        return Unit.Value;
    }
}

