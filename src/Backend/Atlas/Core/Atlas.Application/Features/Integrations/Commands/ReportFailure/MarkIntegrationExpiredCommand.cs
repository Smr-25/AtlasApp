using MediatR;

namespace Atlas.Application.Features.Integrations.Commands.ReportFailure;

public record MarkIntegrationExpiredCommand(Guid IntegrationId) : IRequest;