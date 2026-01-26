using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.SetTelegramChatId;

public record SetTelegramChatIdCommand(
    Guid UserId,
    string TelegramChatId
) : IRequest<ResponseModel<bool>>;