using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Integrations.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Integrations.Queries.GetPendingIntegrations;

public class GetPendingIntegrationsQueryHandler(
    IApplicationDbContext context,
    IMapper mapper,
    ICurrentUserService currentUserService,
    ILogger<GetPendingIntegrationsQueryHandler> logger)
    : IRequestHandler<GetPendingIntegrationsQuery, List<IntegrationDto>>
{
    public async Task<List<IntegrationDto>> Handle(GetPendingIntegrationsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogDebug("Fetching pending integrations for user {UserId}", userId);

        var integrations = await context.Integrations
            .Where(x => x.UserProfileId == userId && !x.IsDeleted && x.Status == IntegrationStatus.PendingSetup)
            .ProjectTo<IntegrationDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        logger.LogDebug("Retrieved {Count} pending integrations for user {UserId}", integrations.Count, userId);
        return integrations;
    }
}

