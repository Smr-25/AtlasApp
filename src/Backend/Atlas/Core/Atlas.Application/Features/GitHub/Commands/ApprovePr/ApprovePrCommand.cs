using MediatR;

namespace Atlas.Application.Features.GitHub.Commands.ApprovePr;

public record ApprovePrCommand(Guid IntegrationId, string Owner, string Repo, string PrNumber) : IRequest;

