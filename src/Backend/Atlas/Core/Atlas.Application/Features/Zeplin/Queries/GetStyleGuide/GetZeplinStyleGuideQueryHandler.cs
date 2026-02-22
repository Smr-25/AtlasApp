using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Zeplin.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Zeplin.Queries.GetStyleGuide;

public class GetZeplinStyleGuideQueryHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    IZeplinAdapter zeplinAdapter
) : IRequestHandler<GetZeplinStyleGuideQuery, ZeplinStyleGuideDto>
{
    public async Task<ZeplinStyleGuideDto> Handle(GetZeplinStyleGuideQuery request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken);
        return await zeplinAdapter.GetStyleGuideAsync(token, request.ProjectId, cancellationToken);
    }
}

