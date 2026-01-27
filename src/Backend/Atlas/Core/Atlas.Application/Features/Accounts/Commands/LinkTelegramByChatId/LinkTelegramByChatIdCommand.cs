using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.LinkTelegramByChatId;

public record LinkTelegramByChatIdCommand(
    string LinkCode,
    string ChatId
) : IRequest<Unit>;