using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Figma.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Figma.Queries.GetComments;

public class GetFigmaCommentsQueryHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    IFigmaAdapter figmaAdapter
) : IRequestHandler<GetFigmaCommentsQuery, List<FigmaCommentDto>>
{
    public async Task<List<FigmaCommentDto>> Handle(GetFigmaCommentsQuery request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken);
        return await figmaAdapter.GetCommentsAsync(token, request.FileKey, cancellationToken);
    }
}

