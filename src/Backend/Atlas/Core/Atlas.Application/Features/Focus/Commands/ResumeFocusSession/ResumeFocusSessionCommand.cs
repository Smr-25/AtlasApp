using MediatR;

namespace Atlas.Application.Features.Focus.Commands.ResumeFocusSession;

public record ResumeFocusSessionCommand(Guid SessionId) : IRequest<bool>;

