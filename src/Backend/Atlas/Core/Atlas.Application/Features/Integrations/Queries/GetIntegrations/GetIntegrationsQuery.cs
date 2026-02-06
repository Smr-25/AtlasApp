using Atlas.Application.Features.Integrations.Dtos;
using MediatR;

namespace Atlas.Application.Features.Integrations.Queries.GetIntegrations;

public record GetIntegrationsQuery : IRequest<List<IntegrationDto>>;
