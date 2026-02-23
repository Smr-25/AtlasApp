using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderUtilities.Queries.EstimateCost;

public record EstimateCostQuery(double HoursEstimated, double HourlyRate, double ServerMonthlyCost, int EstimatedMonths) : IRequest<CostEstimateResult>;

