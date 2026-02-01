using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.DeleteAccount;

public record DeleteAccountCommand(
) : IRequest<bool>;
