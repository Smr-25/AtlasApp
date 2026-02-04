using Atlas.Application.Features.Focus.Dtos;
using MediatR;

namespace Atlas.Application.Features.Focus.Queries.GetFocusStats;

public record GetFocusStatsQuery : IRequest<FocusStatsDto>;