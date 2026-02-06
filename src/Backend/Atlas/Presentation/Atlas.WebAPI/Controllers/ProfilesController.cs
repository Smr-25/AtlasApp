using System.Security.Claims;
using Atlas.Application.Features.Profiles.Queries.GetUserProfile;
using Atlas.Application.Features.Profiles.UpdateProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize] 
public class ProfilesController : ApiControllerBase
{
    
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdString == null) return Unauthorized();
        
        var userId = Guid.Parse(userIdString);

        var result = await Mediator.Send(new GetUserProfileQuery(userId));
        return Ok(result);
    }

   
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile(UpdateUserProfileCommand command)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdString == null) return Unauthorized();
        
        var safeCommand = command with { UserId = Guid.Parse(userIdString) };
        await Mediator.Send(safeCommand);
        return NoContent();
    }
}