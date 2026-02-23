using MediatR;

namespace Atlas.Application.Features.SecOpsUtilities.Commands.EncodePayload;

public record EncodePayloadCommand(string Input, string Encoding = "Base64") : IRequest<string>;

