using MediatR;

namespace Atlas.Application.Features.DevInsights.Queries.GetDeploymentSuccessRate;

public record GetDeploymentSuccessRateQuery(DateTime From, DateTime To) : IRequest<DeploymentSuccessRateResult>;

public record DeploymentSuccessRateResult(double SuccessRate, int TotalDeployments, int SuccessfulDeployments);

