using MediatR;

namespace Atlas.Application.Features.Focus.Commands.CompleteFocusSession;

public record CompleteFocusSessionCommand(Guid SessionId) : IRequest<bool>;

