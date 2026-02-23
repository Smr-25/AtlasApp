using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderAgents.Queries.CatchUnassignedBugs;

public record CatchUnassignedBugsQuery(Guid TeamId) : IRequest<UnassignedBugResult>;

