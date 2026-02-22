using Atlas.Application.Features.Aws.Queries.GetDeployments;
using Atlas.Application.Features.Aws.Queries.GetDeploymentStatus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class AwsController : ApiControllerBase
{
    [HttpGet("{integrationId}/deployments")]
    public async Task<IActionResult> GetDeployments(Guid integrationId, [FromQuery] string serviceName)
    {
        var result = await Mediator.Send(new GetDeploymentsQuery(integrationId, serviceName));
        return OkResponse(result);
    }

    [HttpGet("{integrationId}/deployments/{deploymentId}/status")]
    public async Task<IActionResult> GetDeploymentStatus(Guid integrationId, string deploymentId)
    {
        var result = await Mediator.Send(new GetDeploymentStatusQuery(integrationId, deploymentId));
        return OkResponse(result);
    }
}

