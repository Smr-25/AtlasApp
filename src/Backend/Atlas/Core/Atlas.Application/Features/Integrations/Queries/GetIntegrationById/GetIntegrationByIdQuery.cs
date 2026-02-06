using Atlas.Application.Features.Integrations.Dtos;
using MediatR;

namespace Atlas.Application.Features.Integrations.Queries.GetIntegrationById;

public record GetIntegrationByIdQuery(Guid IntegrationId) : IRequest<IntegrationDto>;

