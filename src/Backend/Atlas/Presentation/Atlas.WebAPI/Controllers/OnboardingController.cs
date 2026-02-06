using Atlas.Application.Features.Onboarding.Commands.AddOption;
using Atlas.Application.Features.Onboarding.Commands.CompleteOnboarding;
using Atlas.Application.Features.Onboarding.Commands.CreateQuestion;
using Atlas.Application.Features.Onboarding.Queries.GetQuestions;
using Atlas.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

public class OnboardingController : ApiControllerBase
{
    [HttpPost("questions")]
    public async Task<IActionResult> CreateQuestion(CreateOnboardingQuestionCommand command)
    {
        var id = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetQuestions), new { profession = 0 }, id);
    }

    [HttpPost("questions/{id}/options")]
    public async Task<IActionResult> AddOption(Guid id, AddOnboardingOptionCommand command)
    {
        if (id != command.QuestionId) return BadRequest();
        
        var optionId = await Mediator.Send(command);
        return Ok(optionId);
    }

    [HttpGet("questions")]
    [AllowAnonymous] 
    public async Task<IActionResult> GetQuestions([FromQuery] UserProfession profession)
    {
        var result = await Mediator.Send(new GetOnboardingQuestionsQuery(profession));
        return Ok(result);
    }

   
    [HttpPost("complete")]
    public async Task<IActionResult> Complete(CompleteOnboardingCommand command)
    {
        var profileId = await Mediator.Send(command);
        return Ok(new { ProfileId = profileId });
    }
}