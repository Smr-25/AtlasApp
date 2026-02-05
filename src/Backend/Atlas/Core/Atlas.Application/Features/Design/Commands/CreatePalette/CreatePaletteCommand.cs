using Atlas.Application.Common.Exceptions.Users;
using MediatR;

namespace Atlas.Application.Features.Design.Commands.CreatePalette;

public record CreatePaletteCommand(string Name) : IRequest<Guid>;