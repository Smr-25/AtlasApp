using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;

namespace Atlas.Application.Features.Design.Commands.CreatePalette;

public class CreatePaletteCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService
) : IRequestHandler<CreatePaletteCommand, Guid>
{
    public async Task<Guid> Handle(CreatePaletteCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        var palette = DesignPalette.Create(userId, request.Name);
        await context.DesignPalettes.AddAsync(palette, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return palette.Id;
    }
}