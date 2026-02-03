using Atlas.Application.Features.Dashboard.Dtos;
using MediatR;

namespace Atlas.Application.Features.Dashboard.Queries.GetDashboardStats;

public record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;