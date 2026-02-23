using Atlas.Application.Features.TeamInfo.Commands.ManageVaultLink;
using Atlas.Application.Features.TeamInfo.Commands.SetTeamObjective;
using Atlas.Application.Features.TeamInfo.Commands.UpdateMemberFocus;
using Atlas.Application.Features.TeamInfo.Commands.UpsertTeamArmory;
using Atlas.Application.Features.TeamInfo.Queries.GetTeamInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class TeamInfoController : ApiControllerBase
{
    [HttpGet("{teamId}")]
    public async Task<IActionResult> GetTeamInfo(Guid teamId)
    {
        var result = await Mediator.Send(new GetTeamInfoQuery(teamId));
        return OkResponse(result);
    }

    [HttpPost("{teamId}/objective")]
    public async Task<IActionResult> SetObjective(Guid teamId, [FromBody] SetObjectiveRequest request)
    {
        var id = await Mediator.Send(new SetTeamObjectiveCommand(teamId, request.Title, request.Description, request.Deadline));
        return CreatedResponse(id);
    }

    [HttpPut("{teamId}/my-focus")]
    public async Task<IActionResult> UpdateMyFocus(Guid teamId, [FromBody] UpdateFocusRequest request)
    {
        var id = await Mediator.Send(new UpdateMemberFocusCommand(teamId, request.FocusDescription));
        return OkResponse(id);
    }

    [HttpPut("{teamId}/armory")]
    public async Task<IActionResult> UpsertArmory(Guid teamId, [FromBody] UpsertArmoryRequest request)
    {
        var id = await Mediator.Send(new UpsertTeamArmoryCommand(
            teamId, request.StagingServerUrl, request.TestAccountEmail,
            request.TestAccountPassword, request.ProductionVersion, request.StagingVersion));
        return OkResponse(id);
    }

    [HttpPost("{teamId}/vault-links")]
    public async Task<IActionResult> AddVaultLink(Guid teamId, [FromBody] AddVaultLinkRequest request)
    {
        var id = await Mediator.Send(new AddVaultLinkCommand(teamId, request.Label, request.Url, request.Icon, request.SortOrder));
        return CreatedResponse(id);
    }

    [HttpPut("{teamId}/vault-links/{linkId}")]
    public async Task<IActionResult> UpdateVaultLink(Guid teamId, Guid linkId, [FromBody] UpdateVaultLinkRequest request)
    {
        await Mediator.Send(new UpdateVaultLinkCommand(teamId, linkId, request.Label, request.Url, request.Icon, request.SortOrder));
        return NoContentResponse();
    }

    [HttpDelete("{teamId}/vault-links/{linkId}")]
    public async Task<IActionResult> DeleteVaultLink(Guid teamId, Guid linkId)
    {
        await Mediator.Send(new DeleteVaultLinkCommand(teamId, linkId));
        return NoContentResponse();
    }
}

public record SetObjectiveRequest(string Title, string? Description, DateTime? Deadline);
public record UpdateFocusRequest(string FocusDescription);
public record UpsertArmoryRequest(string StagingServerUrl, string? TestAccountEmail, string? TestAccountPassword, string? ProductionVersion, string? StagingVersion);
public record AddVaultLinkRequest(string Label, string Url, string? Icon, int SortOrder = 0);
public record UpdateVaultLinkRequest(string Label, string Url, string? Icon, int SortOrder);

