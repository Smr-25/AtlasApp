using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Aws.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Aws.Queries.GetDeployments;

public class GetDeploymentsQueryHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    IAwsAdapter awsAdapter
) : IRequestHandler<GetDeploymentsQuery, List<AwsDeploymentDto>>
{
    public async Task<List<AwsDeploymentDto>> Handle(GetDeploymentsQuery request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken);
        return await awsAdapter.GetDeploymentsAsync(token, request.ServiceName, cancellationToken);
    }
}

