using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerAgents.Queries.DetectCartAbandonment;

public class DetectCartAbandonmentQueryHandler(
    IMarketerAgentService agentService,
    ICurrentUserService currentUser
) : IRequestHandler<DetectCartAbandonmentQuery, CartAbandonmentResult>
{
    public async Task<CartAbandonmentResult> Handle(DetectCartAbandonmentQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await agentService.DetectCartAbandonmentAsync(userId, cancellationToken);
    }
}

