using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Figma.Commands.ResolveComment;

public class ResolveFigmaCommentCommandHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    IFigmaAdapter figmaAdapter
) : IRequestHandler<ResolveFigmaCommentCommand>
{
    public async Task Handle(ResolveFigmaCommentCommand request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken);
        await figmaAdapter.ResolveCommentAsync(token, request.FileKey, request.CommentId, cancellationToken);
    }
}

