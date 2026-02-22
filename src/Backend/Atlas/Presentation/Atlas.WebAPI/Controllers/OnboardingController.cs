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
            return NotFoundResponse("Profession question not found");
        return OkResponse(result);
    }
    [HttpGet("questions")]
    [AllowAnonymous] 
    public async Task<IActionResult> GetQuestions([FromQuery] UserProfession? profession = null)
    {
        var result = await Mediator.Send(new GetOnboardingQuestionsQuery(profession));
        return OkResponse(result);
    }

    [HttpPost("questions")]
    public async Task<IActionResult> CreateQuestion(CreateOnboardingQuestionCommand command)
    {
        var id = await Mediator.Send(command);
        return CreatedResponse(id);
    }

    [HttpPost("questions/{id}/options")]
    public async Task<IActionResult> AddOption(Guid id, AddOnboardingOptionCommand command)
    {
        if (id != command.QuestionId) return BadRequestResponse("Question ID mismatch.");
        
        var optionId = await Mediator.Send(command);
        return OkResponse(optionId);
    }

    [HttpPost("complete")]
    public async Task<IActionResult> Complete(CompleteOnboardingCommand command)
    {
        var profileId = await Mediator.Send(command);
        return OkResponse(new { ProfileId = profileId });
    }
}