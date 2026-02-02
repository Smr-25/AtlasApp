using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Atlas.Application.Features.Workspaces.Commands.UpdateLinkConfig;

public class UpdateLinkConfigCommandHandler(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
    : IRequestHandler<UpdateLinkConfigCommand, bool>
{
    public async Task<bool> Handle(UpdateLinkConfigCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        var link = await applicationDbContext.WorkspaceIntegrations
            .Include(wi => wi.Workspace).ThenInclude(w => w.Persona)
            .FirstOrDefaultAsync(wi => wi.Id == request.LinkId && wi.Workspace.Persona.UserId.Equals(userId), cancellationToken);

        if (link == null) throw new NotFoundException("Link not found.");
        
        var jsonConfig = JsonConvert.SerializeObject(request.ConfigData);
        link.UpdateConfig(jsonConfig);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}