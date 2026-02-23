using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderAgents.Commands.NagPrReviews;

public record NagPrReviewsCommand(Guid TeamId, int ThresholdHours) : IRequest<PrReviewNagResult>;

