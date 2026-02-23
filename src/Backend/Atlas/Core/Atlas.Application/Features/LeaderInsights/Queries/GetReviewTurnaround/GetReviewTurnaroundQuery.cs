using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderInsights.Queries.GetReviewTurnaround;

public record GetReviewTurnaroundQuery(Guid TeamId, DateTime From, DateTime To) : IRequest<ReviewTurnaroundResult>;

