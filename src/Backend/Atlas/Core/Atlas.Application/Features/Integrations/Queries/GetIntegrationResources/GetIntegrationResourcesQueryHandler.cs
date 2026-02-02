using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Integrations.Queries.GetIntegrationResources;

public class GetIntegrationResourcesQueryHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IEncryptionService encryptionService,
    IEnumerable<IIntegrationAdapter> adapters)
    : IRequestHandler<GetIntegrationResourcesQuery, List<ExternalResourceDto>>
{

    public async Task<List<ExternalResourceDto>> Handle(GetIntegrationResourcesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        var integration = await applicationDbContext.Integrations
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.IntegrationId && i.Persona.UserId.Equals(userId),
                cancellationToken);

        if (integration == null)
            throw new NotFoundException("Integration not found.");

        var accessToken = encryptionService.Decrypt(integration.EncryptedAccessToken!);
        if (string.IsNullOrEmpty(accessToken)) return [];

        var adapter = adapters.FirstOrDefault(a => a.Provider == integration.Provider);

        if (adapter == null)
            throw new NotSupportedException($"Provider {integration.Provider} is not supported yet.");

        return await adapter.GetResourcesAsync(accessToken, cancellationToken);
    }
}