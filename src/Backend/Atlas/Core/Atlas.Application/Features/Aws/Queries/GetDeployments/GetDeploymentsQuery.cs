using Atlas.Application.Features.Aws.Dtos;
using MediatR;

namespace Atlas.Application.Features.Aws.Queries.GetDeployments;

public record GetDeploymentsQuery(Guid IntegrationId, string ServiceName) : IRequest<List<AwsDeploymentDto>>;

