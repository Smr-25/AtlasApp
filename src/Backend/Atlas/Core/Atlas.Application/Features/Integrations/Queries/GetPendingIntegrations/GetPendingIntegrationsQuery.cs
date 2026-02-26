using Atlas.Application.Features.Integrations.Dtos;
using MediatR;

namespace Atlas.Application.Features.Integrations.Queries.GetPendingIntegrations;

public record GetPendingIntegrationsQuery : IRequest<List<IntegrationDto>>;

