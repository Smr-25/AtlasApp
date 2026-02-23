using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderAgents.Queries.CheckMilestoneCelebration;

public record CheckMilestoneCelebrationQuery(Guid TeamId) : IRequest<MilestoneCelebrationResult>;

