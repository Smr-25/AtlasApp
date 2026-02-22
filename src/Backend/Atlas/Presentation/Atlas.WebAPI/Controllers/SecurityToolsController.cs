using Atlas.Application.Features.SecurityTools.Queries.ScanVulnerabilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class SecurityToolsController : ApiControllerBase
{
    [HttpPost("scan-dependencies")]
    public async Task<IActionResult> ScanDependencies([FromBody] ScanVulnerabilitiesQuery query)
    {
        try
        {
            var result = await Mediator.Send(query);
            if (result.Count == 0 || result.All(r => r.Vulnerabilities.Count == 0))
                return OkResponse(new { message = "No vulnerabilities found! Your project is secure. 🛡️" });
            

            return OkResponse(result);
        }
        catch (Exception ex)
        {
            var obj = new { error = "Security scan failed.", details = ex.Message };
            return StatusCode(500, obj);
        }
    }
}