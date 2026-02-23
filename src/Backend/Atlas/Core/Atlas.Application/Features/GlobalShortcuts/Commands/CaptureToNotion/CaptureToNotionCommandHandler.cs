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

        var profile = await dbContext.UserProfiles
            .FirstOrDefaultAsync(p => p.Id == userId, cancellationToken);

        if (profile != null)
        {
            var notionIntegration = await dbContext.Integrations
                .FirstOrDefaultAsync(i =>
                    i.UserProfileId == profile.Id &&
                    i.Provider == IntegrationProvider.Notion &&
                    i.Status == IntegrationStatus.Active, cancellationToken);

            if (notionIntegration != null)
            {
                var externalId = await notionService.SendSnippetToNotionAsync(
                    request.Title ?? "Quick Capture",
                    request.Content,
                    "text",
                    notionIntegration.MetadataJson ?? "",
                    notionIntegration.EncryptedAccessToken,
                    cancellationToken);

                capture.MarkSynced(externalId);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return capture.Id;
    }
}

