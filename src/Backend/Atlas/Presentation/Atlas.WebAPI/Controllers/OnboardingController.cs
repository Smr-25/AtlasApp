using Atlas.Application.Features.Onboarding.Commands.AddOption;
using Atlas.Application.Features.Onboarding.Commands.CompleteOnboarding;
using Atlas.Application.Features.Onboarding.Commands.CreateQuestion;
using Atlas.Application.Features.Onboarding.Queries.GetProfessionQuestion;
using Atlas.Application.Features.Onboarding.Queries.GetQuestions;
using Atlas.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

public class OnboardingController : ApiControllerBase
{
    [HttpGet("profession-question")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProfessionQuestion()
    {
        var result = await Mediator.Send(new GetProfessionQuestionQuery());
        if (result == null)
            return NotFound("Profession question not found");
        return Ok(result);
    }
    [HttpGet("questions")]
    [AllowAnonymous] 
    public async Task<IActionResult> GetQuestions([FromQuery] UserProfession? profession = null)
    {
        var result = await Mediator.Send(new GetOnboardingQuestionsQuery(profession));
        return Ok(result);
    }

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

    [HttpPost("complete")]
    public async Task<IActionResult> Complete(CompleteOnboardingCommand command)
    {
        var profileId = await Mediator.Send(command);
        return Ok(new { ProfileId = profileId });
    }
}