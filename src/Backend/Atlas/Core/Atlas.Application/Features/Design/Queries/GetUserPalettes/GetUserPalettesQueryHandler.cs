using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Design.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Design.Queries.GetUserPalettes;

public class GetUserPalettesQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService
) : IRequestHandler<GetUserPalettesQuery, List<DesignPaletteDto>>
{
    public async Task<List<DesignPaletteDto>> Handle(GetUserPalettesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        return await context.DesignPalettes
            .Where(p => p.UserId == userId)
            .Include(p => p.Colors)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new DesignPaletteDto(
                p.Id,
                p.Name,
                p.Colors.Select(c => new PaletteColorDto(c.Id, c.Name, c.HexCode)).ToList()
            ))
            .ToListAsync(cancellationToken);
    }
}