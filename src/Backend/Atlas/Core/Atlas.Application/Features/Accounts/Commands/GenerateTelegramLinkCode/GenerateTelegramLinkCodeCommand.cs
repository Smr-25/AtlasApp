using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.GenerateTelegramLinkCode;

public record GenerateTelegramLinkCodeCommand(
    Guid UserId
) : IRequest<ResponseModel<string>>;  