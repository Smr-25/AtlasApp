using MediatR;

namespace Atlas.Application.Features.Design.Commands.AddColor;

public record AddColorCommand(Guid PaletteId, string Name, string HexCode) : IRequest<Guid>;