using Atlas.Application.Features.Onboarding.Commands.CompleteOnboarding;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

public class OnboardingController : ApiControllerBase
{

    [HttpPost("complete")]
    public async Task<IActionResult> Complete(CompleteOnboardingCommand command)
    {
        var profileId = await Mediator.Send(command);
        return OkResponse(new { ProfileId = profileId });
    }
}