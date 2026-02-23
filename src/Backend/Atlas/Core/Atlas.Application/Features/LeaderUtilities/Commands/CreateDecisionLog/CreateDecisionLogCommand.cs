using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderUtilities.Commands.CreateDecisionLog;

public record CreateDecisionLogCommand(string Decision, string Rationale, string DecidedBy) : IRequest<DecisionLogEntry>;

