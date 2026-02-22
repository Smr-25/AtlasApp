using Atlas.Application.Features.Dribbble.Dtos;
using MediatR;

namespace Atlas.Application.Features.Dribbble.Queries.GetInspiration;

public record GetDribbbleInspirationQuery(Guid IntegrationId, string? SearchQuery) : IRequest<List<DribbbleShotDto>>;

