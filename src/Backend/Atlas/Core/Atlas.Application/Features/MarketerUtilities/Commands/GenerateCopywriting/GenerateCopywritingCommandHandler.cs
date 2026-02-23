using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerUtilities.Commands.GenerateCopywriting;

public class GenerateCopywritingCommandHandler(
    IMarketerUtilityService marketerUtility
) : IRequestHandler<GenerateCopywritingCommand, string>
{
    public async Task<string> Handle(GenerateCopywritingCommand request, CancellationToken cancellationToken)
    {
        return await marketerUtility.GenerateCopywritingAsync(request.ProductName, request.Tone, cancellationToken);
    }
}

