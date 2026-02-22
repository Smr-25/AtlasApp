using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Aws.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Aws.Queries.GetDeploymentStatus;

public class GetDeploymentStatusQueryHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    IAwsAdapter awsAdapter
) : IRequestHandler<GetDeploymentStatusQuery, AwsDeploymentStatusDto>
{
    public async Task<AwsDeploymentStatusDto> Handle(GetDeploymentStatusQuery request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken);
        return await awsAdapter.GetDeploymentStatusAsync(token, request.DeploymentId, cancellationToken);
    }
}

