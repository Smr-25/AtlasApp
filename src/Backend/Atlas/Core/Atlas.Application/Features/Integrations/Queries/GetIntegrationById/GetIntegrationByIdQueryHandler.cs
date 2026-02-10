using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Integrations.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Integrations.Queries.GetIntegrationById;

public class GetIntegrationByIdQueryHandler(
    IApplicationDbContext context,
    IMapper mapper,
    ICurrentUserService currentUserService,
    ILogger<GetIntegrationByIdQueryHandler> logger) : IRequestHandler<GetIntegrationByIdQuery, IntegrationDto>
{
    public async Task<IntegrationDto> Handle(GetIntegrationByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogDebug("Fetching integration {IntegrationId} for user {UserId}", request.IntegrationId, userId);

        var integration = await context.Integrations
            .Where(i => i.Id == request.IntegrationId && i.UserProfileId == userId && !i.IsDeleted)
            .ProjectTo<IntegrationDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (integration == null) throw new NotFoundException("Integration", request.IntegrationId);

        logger.LogDebug("Successfully retrieved integration {IntegrationId}", request.IntegrationId);
        return integration;
    }
}

