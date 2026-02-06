using Atlas.Application.Features.Projects.Commands.AddMigration;
using Atlas.Application.Features.Projects.Commands.CreateProject;
using Atlas.Application.Features.Projects.Commands.UpdateDatabase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class ProjectsController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateProjectCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("{id}/migration")]
    public async Task<ActionResult<string>> AddMigration(Guid id, [FromBody] string? name)
    {
        return await Mediator.Send(new AddMigrationCommand(id, name));
    }

    [HttpPost("{id}/database-update")]
    public async Task<ActionResult<string>> UpdateDatabase(Guid id, [FromBody] string? targetMigration)
    {
        return await Mediator.Send(new UpdateDatabaseCommand(id, targetMigration));
    }
}