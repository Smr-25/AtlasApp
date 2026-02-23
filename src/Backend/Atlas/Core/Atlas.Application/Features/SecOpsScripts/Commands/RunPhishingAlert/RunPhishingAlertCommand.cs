using MediatR;

namespace Atlas.Application.Features.SecOpsScripts.Commands.RunPhishingAlert;

public record RunPhishingAlertCommand(string EmailHeaders, string SenderAddress) : IRequest<string>;

