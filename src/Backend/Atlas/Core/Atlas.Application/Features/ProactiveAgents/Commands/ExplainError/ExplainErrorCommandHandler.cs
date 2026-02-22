using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.ProactiveAgents.Commands.ExplainError;

public class ExplainErrorCommandHandler(
    IProactiveAgentService agentService
) : IRequestHandler<ExplainErrorCommand, string>
{
    public async Task<string> Handle(ExplainErrorCommand request, CancellationToken cancellationToken)
    {
        return await agentService.ExplainErrorAsync(request.ErrorMessage, request.StackTrace, cancellationToken);
    }
}

