using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Webhooks.Commands.ToggleWebhook;

public record ToggleWebhookCommand(Guid WebhookId, bool Active) : IRequest;

public class ToggleWebhookCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<ToggleWebhookCommand>
{
    public async Task Handle(ToggleWebhookCommand request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();
        var webhook = await context.OutgoingWebhooks
            .FirstOrDefaultAsync(w => w.Id == request.WebhookId && w.UserId == userId, ct)
            ?? throw new NotFoundException("Webhook", request.WebhookId);

        webhook.SetActive(request.Active);
        await context.SaveChangesAsync(ct);
    }
}

