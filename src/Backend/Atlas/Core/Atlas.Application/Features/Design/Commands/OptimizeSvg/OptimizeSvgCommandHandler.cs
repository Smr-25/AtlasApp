using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Design.Commands.OptimizeSvg;

public class OptimizeSvgCommandHandler(
    IDesignUtilityService designUtility
) : IRequestHandler<OptimizeSvgCommand, OptimizeSvgResult>
{
    public Task<OptimizeSvgResult> Handle(OptimizeSvgCommand request, CancellationToken cancellationToken)
    {
        var optimized = designUtility.OptimizeSvg(request.SvgContent);
        return Task.FromResult(new OptimizeSvgResult(optimized, request.SvgContent.Length, optimized.Length));
    }
}

