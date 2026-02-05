using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Design.Commands.AddColor;

public class AddColorCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService
) : IRequestHandler<AddColorCommand, Guid>
{
    public async Task<Guid> Handle(AddColorCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        var palette = await context.DesignPalettes
            .Include(p => p.Colors)
            .FirstOrDefaultAsync(p => p.Id == request.PaletteId && p.UserId == userId, cancellationToken);
        if (palette == null) 
            throw new NotFoundException("Palette", request.PaletteId);

        palette.AddColor(request.Name, request.HexCode);
        await context.SaveChangesAsync(cancellationToken);
        return palette.Colors.Last().Id;
    }
}