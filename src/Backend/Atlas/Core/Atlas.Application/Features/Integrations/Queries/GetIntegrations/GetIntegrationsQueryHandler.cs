using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Integrations.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Integrations.Queries.GetIntegrations;

public class GetIntegrationsQueryHandler(
    IApplicationDbContext context, 
    IMapper mapper,
    ICurrentUserService currentUserService,
    ILogger<GetIntegrationsQueryHandler> logger) 
    : IRequestHandler<GetIntegrationsQuery, List<IntegrationDto>>
{
    public async Task<List<IntegrationDto>> Handle(GetIntegrationsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogDebug("Fetching integrations for user {UserId}", userId);
        
        var integrations = await context.Integrations
            .Where(x => x.UserProfileId == userId && !x.IsDeleted)
            .ProjectTo<IntegrationDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        
        logger.LogDebug("Retrieved {Count} integrations for user {UserId}", integrations.Count, userId);
        return integrations;
    }
}

