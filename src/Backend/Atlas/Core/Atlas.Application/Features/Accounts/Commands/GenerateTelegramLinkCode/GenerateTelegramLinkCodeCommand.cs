using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.GenerateTelegramLinkCode;

public record GenerateTelegramLinkCodeCommand() : IRequest<string>;
