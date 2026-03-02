using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Dribbble.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Dribbble.Queries.GetInspiration;

public class GetDribbbleInspirationQueryHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    IDribbbleAdapter dribbbleAdapter
) : IRequestHandler<GetDribbbleInspirationQuery, List<DribbbleShotDto>>
{
    public async Task<List<DribbbleShotDto>> Handle(GetDribbbleInspirationQuery request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken!);

        return string.IsNullOrEmpty(request.SearchQuery)
            ? await dribbbleAdapter.GetShotsAsync(token, cancellationToken)
            : await dribbbleAdapter.SearchInspirationAsync(token, request.SearchQuery, cancellationToken);
    }
}

