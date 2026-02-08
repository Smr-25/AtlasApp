using MediatR;

namespace Atlas.Application.Features.GitHub.Commands.MergePr;

public record MergePrCommand(Guid IntegrationId, string Owner, string Repo, string PrNumber) : IRequest;
