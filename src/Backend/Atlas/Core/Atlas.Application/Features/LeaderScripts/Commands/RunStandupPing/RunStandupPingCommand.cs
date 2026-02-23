using MediatR;

namespace Atlas.Application.Features.LeaderScripts.Commands.RunStandupPing;

public record RunStandupPingCommand(Guid TeamId) : IRequest<string>;

