using MediatR;

namespace Atlas.Application.Features.SecOpsScripts.Commands.RunRotateSsh;

public record RunRotateSshCommand(string KeyComment, int KeySize = 4096) : IRequest<string>;

