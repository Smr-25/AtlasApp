using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Design.Commands.ExtractCssVars;

public record ExtractCssVarsCommand(List<ColorVariable> Colors, string Format = "css") : IRequest<string>;

