using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DevUtilities.Commands.GenerateSshKey;

public record GenerateSshKeyCommand(string Comment, int KeySize = 4096) : IRequest<SshKeyPairResult>;

