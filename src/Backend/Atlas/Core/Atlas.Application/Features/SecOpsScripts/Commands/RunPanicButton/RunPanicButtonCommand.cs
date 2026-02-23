using MediatR;

namespace Atlas.Application.Features.SecOpsScripts.Commands.RunPanicButton;

public record RunPanicButtonCommand(string? InterfaceName) : IRequest<string>;

