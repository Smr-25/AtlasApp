using MediatR;

namespace Atlas.Application.Features.SecOpsUtilities.Commands.GenerateHash;

public record GenerateHashCommand(string Input, string Algorithm = "SHA256") : IRequest<string>;

