using Atlas.Application.Features.Profiles.Commands.UpdateProfile;
using Atlas.Application.Features.Profiles.Queries.GetUserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize] 
public class ProfilesController : ApiControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var result = await Mediator.Send(new GetUserProfileQuery());
        return Ok(result);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile(UpdateUserProfileCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }
}