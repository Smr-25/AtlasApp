using Atlas.Application.Features.GlobalShortcuts.Dtos;
using MediatR;

namespace Atlas.Application.Features.GlobalShortcuts.Queries.SearchCommandPalette;

public record SearchCommandPaletteQuery(string SearchTerm) : IRequest<CommandPaletteResultDto>;

