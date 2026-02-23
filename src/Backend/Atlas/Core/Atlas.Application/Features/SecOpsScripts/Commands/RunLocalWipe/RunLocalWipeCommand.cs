using MediatR;

namespace Atlas.Application.Features.SecOpsScripts.Commands.RunLocalWipe;

public record RunLocalWipeCommand(bool WipeHistory, bool WipeCredentials) : IRequest<string>;

