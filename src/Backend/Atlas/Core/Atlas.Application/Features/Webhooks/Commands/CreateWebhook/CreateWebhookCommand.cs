using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Webhooks.Dtos;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Webhooks.Commands.CreateWebhook;

public record CreateWebhookCommand(
    string Name, string Url, string? Secret,
    WebhookEvent[] Events, Guid? WorkspaceId = null
) : IRequest<Guid>;

public class CreateWebhookCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<CreateWebhookCommand, Guid>
{
    public async Task<Guid> Handle(CreateWebhookCommand request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();
        var webhook = OutgoingWebhook.Create(userId, request.Name, request.Url, request.Secret, request.Events, request.WorkspaceId);
        await context.OutgoingWebhooks.AddAsync(webhook, ct);
        await context.SaveChangesAsync(ct);
        return webhook.Id;
    }
}

