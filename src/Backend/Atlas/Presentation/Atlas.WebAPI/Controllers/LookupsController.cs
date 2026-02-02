using Atlas.Application.Features.Accounts.Queries;
using Atlas.Application.Features.Accounts.Queries.GetOnboardingQuestions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class LookupsController : ApiControllerBase
{
    [HttpGet("questions/{professionId}")]
    public async Task<IActionResult> GetQuestions(Guid professionId)
    {
        return OkResponse(await Mediator.Send(new GetOnboardingQuestionsQuery(professionId)));
    }
    
    [HttpGet("professions")]
    public async Task<IActionResult> GetProfessions()
    {
        return OkResponse(await Mediator.Send(new GetProfessionsQuery()));
    }
}