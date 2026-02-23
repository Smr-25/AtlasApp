using MediatR;

namespace Atlas.Application.Features.SecOpsUtilities.Commands.SpoofMac;

public record SpoofMacCommand(string InterfaceName = "en0") : IRequest<string>;

