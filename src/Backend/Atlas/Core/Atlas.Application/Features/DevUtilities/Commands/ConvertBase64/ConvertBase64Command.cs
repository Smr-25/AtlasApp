using MediatR;

namespace Atlas.Application.Features.DevUtilities.Commands.ConvertBase64;

public record ConvertBase64Command(string Input, bool Encode) : IRequest<string>;

