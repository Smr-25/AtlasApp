using Atlas.Application.Features.Aws.Dtos;
using MediatR;

namespace Atlas.Application.Features.Aws.Queries.GetDeploymentStatus;

public record GetDeploymentStatusQuery(Guid IntegrationId, string DeploymentId) : IRequest<AwsDeploymentStatusDto>;

