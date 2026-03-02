using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Webhooks.Commands.DeleteWebhook;

public record DeleteWebhookCommand(Guid WebhookId) : IRequest;

public class DeleteWebhookCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<DeleteWebhookCommand>
{
    public async Task Handle(DeleteWebhookCommand request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();
        var webhook = await context.OutgoingWebhooks
            .FirstOrDefaultAsync(w => w.Id == request.WebhookId && w.UserId == userId, ct)
            ?? throw new NotFoundException("Webhook", request.WebhookId);

        webhook.Delete();
        await context.SaveChangesAsync(ct);
    }
}

