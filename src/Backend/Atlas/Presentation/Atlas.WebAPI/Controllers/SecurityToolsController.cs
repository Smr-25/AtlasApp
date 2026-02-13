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
                return Ok(new { message = "No vulnerabilities found! Your project is secure. 🛡️" });
            

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Security scan failed.", details = ex.Message });
        }
    }
}