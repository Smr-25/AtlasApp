using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderScripts.Commands.RunBlockedTaskBlaster;

public record RunBlockedTaskBlasterCommand(Guid TeamId) : IRequest<BlockedTaskBlasterResult>;

