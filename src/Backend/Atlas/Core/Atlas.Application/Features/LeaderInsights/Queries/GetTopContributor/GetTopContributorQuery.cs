using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderInsights.Queries.GetTopContributor;

public record GetTopContributorQuery(Guid TeamId, DateTime From, DateTime To) : IRequest<TopContributorResult>;

