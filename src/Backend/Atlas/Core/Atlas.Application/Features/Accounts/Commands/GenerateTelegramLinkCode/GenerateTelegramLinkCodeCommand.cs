using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.GenerateTelegramLinkCode;

public record GenerateTelegramLinkCodeCommand(
    string LinkCode,
    string ChatId
) : IRequest<Unit>;