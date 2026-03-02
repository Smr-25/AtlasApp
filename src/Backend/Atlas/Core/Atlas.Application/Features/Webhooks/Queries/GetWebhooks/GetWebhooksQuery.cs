using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Webhooks.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Webhooks.Queries.GetWebhooks;

public record GetWebhooksQuery : IRequest<List<WebhookDto>>;

public class GetWebhooksQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<GetWebhooksQuery, List<WebhookDto>>
{
    public async Task<List<WebhookDto>> Handle(GetWebhooksQuery request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();

        return await context.OutgoingWebhooks
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new WebhookDto(
                w.Id, w.Name, w.Url, w.Events, w.IsActive,
                w.ConsecutiveFailures, w.LastDeliveredAt, w.WorkspaceId, w.CreatedAt))
            .ToListAsync(ct);
    }
}

