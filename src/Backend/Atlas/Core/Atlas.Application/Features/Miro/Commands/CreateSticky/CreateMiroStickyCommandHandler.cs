using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Miro.Commands.CreateSticky;

public class CreateMiroStickyCommandHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    IMiroAdapter miroAdapter
) : IRequestHandler<CreateMiroStickyCommand>
{
    public async Task Handle(CreateMiroStickyCommand request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken);
        await miroAdapter.CreateStickyNoteAsync(token, request.BoardId, request.Content, cancellationToken);
    }
}

