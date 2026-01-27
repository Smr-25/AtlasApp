using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.DeleteAccount;

public record DeleteAccountCommand(
) : IRequest<ResponseModel<bool>>;