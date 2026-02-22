using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Design.Commands.ExtractCssVars;

public class ExtractCssVarsCommandHandler(
    IDesignUtilityService designUtility
) : IRequestHandler<ExtractCssVarsCommand, string>
{
    public Task<string> Handle(ExtractCssVarsCommand request, CancellationToken cancellationToken)
    {
        var result = designUtility.ExtractCssVariables(request.Colors, request.Format);
        return Task.FromResult(result);
    }
}

