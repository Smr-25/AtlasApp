using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsAgents.Commands.KillZombieProcesses;

public record KillZombieProcessesCommand : IRequest<List<ZombieProcessInfo>>;

