using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Miro.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Miro.Queries.GetBoards;

public class GetMiroBoardsQueryHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    IMiroAdapter miroAdapter
) : IRequestHandler<GetMiroBoardsQuery, List<MiroBoardDto>>
{
    public async Task<List<MiroBoardDto>> Handle(GetMiroBoardsQuery request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken);
        return await miroAdapter.GetBoardsAsync(token, cancellationToken);
    }
}

