using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Zeplin.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Zeplin.Queries.GetScreens;

public class GetZeplinScreensQueryHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    IZeplinAdapter zeplinAdapter
) : IRequestHandler<GetZeplinScreensQuery, List<ZeplinScreenDto>>
{
    public async Task<List<ZeplinScreenDto>> Handle(GetZeplinScreensQuery request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken);
        return await zeplinAdapter.GetScreensAsync(token, request.ProjectId, cancellationToken);
    }
}

