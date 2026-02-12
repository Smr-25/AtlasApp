using MediatR;

namespace Atlas.Application.Features.SystemTools.Commands.KillProcess;

public record KillProcessCommand(int Pid) : IRequest<bool>;