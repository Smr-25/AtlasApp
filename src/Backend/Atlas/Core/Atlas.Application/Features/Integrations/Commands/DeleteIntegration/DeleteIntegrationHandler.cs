using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Integrations.Commands.DeleteIntegration;

public class DeleteIntegrationHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService) : IRequestHandler<DeleteIntegrationCommand>
{
    public async Task Handle(DeleteIntegrationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
        var integration = await applicationDbContext.Integrations
            .FirstOrDefaultAsync(i => i.Id == request.IntegrationId && i.UserProfileId == userId, cancellationToken);

        if (integration == null) throw new NotFoundException("Integration", request.IntegrationId);

        integration.Delete();
        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}