using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Webhooks.Commands.UpdateWebhook;

public record UpdateWebhookCommand(Guid WebhookId, string Name, string Url, string? Secret, WebhookEvent[] Events) : IRequest;

public class UpdateWebhookCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateWebhookCommand>
{
    public async Task Handle(UpdateWebhookCommand request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();
        var webhook = await context.OutgoingWebhooks
            .FirstOrDefaultAsync(w => w.Id == request.WebhookId && w.UserId == userId, ct)
            ?? throw new NotFoundException("Webhook", request.WebhookId);

        webhook.Update(request.Name, request.Url, request.Secret, request.Events);
        await context.SaveChangesAsync(ct);
    }
}

