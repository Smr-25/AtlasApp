using MediatR;

namespace Atlas.Application.Features.MarketerScripts.Commands.RunSocialBlast;

public record RunSocialBlastCommand(string Content, List<string> Platforms) : IRequest<string>;

