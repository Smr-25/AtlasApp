using MediatR;

namespace Atlas.Application.Features.Integrations.Commands.DeleteIntegration;

public record DeleteIntegrationCommand(Guid IntegrationId) : IRequest;
