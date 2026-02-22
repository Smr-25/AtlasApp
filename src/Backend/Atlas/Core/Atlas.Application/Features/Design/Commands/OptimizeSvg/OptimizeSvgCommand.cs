using MediatR;

namespace Atlas.Application.Features.Design.Commands.OptimizeSvg;

public record OptimizeSvgCommand(string SvgContent) : IRequest<OptimizeSvgResult>;

public record OptimizeSvgResult(string OptimizedSvg, long OriginalLength, long OptimizedLength);

