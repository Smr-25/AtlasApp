using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Features.Design.Dtos;
using MediatR;

namespace Atlas.Application.Features.Design.Queries.GetUserPalettes;

public record GetUserPalettesQuery : IRequest<List<DesignPaletteDto>>;