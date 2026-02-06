using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Integrations.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Integrations.Queries.GetIntegrations;

public class GetIntegrationsHandler(
    IApplicationDbContext context, 
    IMapper mapper,
    ICurrentUserService currentUserService) 
    : IRequestHandler<GetIntegrationsQuery, List<IntegrationDto>>
{
    public async Task<List<IntegrationDto>> Handle(GetIntegrationsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
        return await context.Integrations
            .Where(x => x.UserProfileId == userId && !x.IsDeleted)
            .ProjectTo<IntegrationDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}