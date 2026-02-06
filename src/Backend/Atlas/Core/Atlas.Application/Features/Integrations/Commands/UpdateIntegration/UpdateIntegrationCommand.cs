using MediatR;

namespace Atlas.Application.Features.Integrations.Commands.UpdateIntegration;

public record UpdateIntegrationCommand(Guid IntegrationId, string Name) : IRequest;
