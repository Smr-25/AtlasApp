using MediatR;

namespace Atlas.Application.Features.GitHub.Commands.RejectPr;

public record RejectPrCommand(Guid IntegrationId, string Owner, string Repo, string PrNumber, string? Reason) : IRequest;

