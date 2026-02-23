using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.GlobalShortcuts.Commands.CaptureToNotion;

public class CaptureToNotionCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    INotionService notionService)
    : IRequestHandler<CaptureToNotionCommand, Guid>
{
    public async Task<Guid> Handle(CaptureToNotionCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var capture = QuickCapture.Create(
            userId,
            request.Content,
            QuickCaptureSource.NotionCapture,
            request.Title,
            request.Url);

        await dbContext.QuickCaptures.AddAsync(capture, cancellationToken);

        var notionIntegration = await dbContext.Integrations
            .FirstOrDefaultAsync(i =>
                i.UserId == userId &&
                i.Provider == IntegrationProvider.Notion &&
                i.Status == IntegrationStatus.Connected, cancellationToken);

        if (notionIntegration != null)
        {
            var externalId = await notionService.SendSnippetToNotionAsync(
                request.Title ?? "Quick Capture",
                request.Content,
                "text",
                notionIntegration.ExternalId!,
                notionIntegration.AccessToken!,
                cancellationToken);

            capture.MarkSynced(externalId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return capture.Id;
    }
}

