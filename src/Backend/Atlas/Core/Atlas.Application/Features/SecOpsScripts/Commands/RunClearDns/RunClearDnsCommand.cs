using MediatR;

namespace Atlas.Application.Features.SecOpsScripts.Commands.RunClearDns;

public record RunClearDnsCommand : IRequest<string>;

