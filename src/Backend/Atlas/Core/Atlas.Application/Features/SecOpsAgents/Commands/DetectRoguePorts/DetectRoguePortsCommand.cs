using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsAgents.Commands.DetectRoguePorts;

public record DetectRoguePortsCommand : IRequest<List<RoguePortInfo>>;

