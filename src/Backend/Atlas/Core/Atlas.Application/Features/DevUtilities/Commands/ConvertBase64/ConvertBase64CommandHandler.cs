using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DevUtilities.Commands.ConvertBase64;

public class ConvertBase64CommandHandler(
    IDevUtilityService devUtility
) : IRequestHandler<ConvertBase64Command, string>
{
    public Task<string> Handle(ConvertBase64Command request, CancellationToken cancellationToken)
    {
        var result = request.Encode
            ? devUtility.EncodeBase64(request.Input)
            : devUtility.DecodeBase64(request.Input);
        return Task.FromResult(result);
    }
}

