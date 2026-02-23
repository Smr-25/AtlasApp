using MediatR;

namespace Atlas.Application.Features.SecOpsScripts.Commands.RunFirewallLockdown;

public record RunFirewallLockdownCommand(List<int>? AllowedPorts) : IRequest<string>;

