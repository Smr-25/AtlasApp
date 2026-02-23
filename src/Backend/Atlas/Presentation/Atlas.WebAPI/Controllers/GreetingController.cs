using Atlas.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[AllowAnonymous]
public class GreetingController(IGreetingService greetingService) : ApiControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery] string userName, [FromQuery] int timezoneOffsetMinutes = 0, [FromQuery] string lang = "en")
    {
        if (string.IsNullOrWhiteSpace(userName))
            return BadRequestResponse("userName is required");

        var message = greetingService.GetLocalizedGreeting(userName, timezoneOffsetMinutes, lang);
        return OkResponse(message);
    }

    [HttpPost]
    public IActionResult Post([FromBody] GreetingRequest? request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.UserName))
            return BadRequestResponse("userName is required in body");

        var message = greetingService.GetLocalizedGreeting(request.UserName, request.TimezoneOffsetMinutes, request.Lang ?? "en");
        return OkResponse(message);
    }
}

public record GreetingRequest(string UserName, int TimezoneOffsetMinutes = 0, string? Lang = "en");
