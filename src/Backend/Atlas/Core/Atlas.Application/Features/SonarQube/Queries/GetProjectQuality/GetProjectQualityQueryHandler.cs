using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.SonarQube.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.SonarQube.Queries.GetProjectQuality;

public class GetProjectQualityQueryHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    ISonarQubeAdapter sonarQubeAdapter
) : IRequestHandler<GetProjectQualityQuery, SonarQubeProjectQualityDto>
{
    public async Task<SonarQubeProjectQualityDto> Handle(GetProjectQualityQuery request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken!);
        return await sonarQubeAdapter.GetProjectQualityAsync(token, request.ProjectKey, cancellationToken);
    }
}

