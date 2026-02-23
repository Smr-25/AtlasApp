using MediatR;

namespace Atlas.Application.Features.MarketerScripts.Commands.RunBulkEmailVerifier;

public record RunBulkEmailVerifierCommand(List<string> Emails) : IRequest<BulkEmailVerifyResult>;

public record BulkEmailVerifyResult(int Total, int Valid, int Invalid, List<string> InvalidEmails);

