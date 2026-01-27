using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.SetTelegramChatId;

public record SetTelegramChatIdCommand(
    string TelegramChatId
) : IRequest<ResponseModel<bool>>;