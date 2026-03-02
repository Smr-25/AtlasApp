using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.LottieFiles.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.LottieFiles.Queries.SearchAnimations;

public class SearchLottieAnimationsQueryHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    ILottieFilesAdapter lottieAdapter
) : IRequestHandler<SearchLottieAnimationsQuery, List<LottieAnimationDto>>
{
    public async Task<List<LottieAnimationDto>> Handle(SearchLottieAnimationsQuery request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken!);
        return await lottieAdapter.SearchAnimationsAsync(token, request.Query, cancellationToken);
    }
}

