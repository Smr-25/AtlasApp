using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Integrations.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Integrations.Queries.GetIntegrationById;

public class GetIntegrationByIdHandler(
    IApplicationDbContext context,
    IMapper mapper,
    ICurrentUserService currentUserService) : IRequestHandler<GetIntegrationByIdQuery, IntegrationDto>
{
    public async Task<IntegrationDto> Handle(GetIntegrationByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var integration = await context.Integrations
            .Where(i => i.Id == request.IntegrationId && i.UserProfileId == userId && !i.IsDeleted)
            .ProjectTo<IntegrationDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (integration == null) throw new NotFoundException("Integration", request.IntegrationId);

        return integration;
    }
}

