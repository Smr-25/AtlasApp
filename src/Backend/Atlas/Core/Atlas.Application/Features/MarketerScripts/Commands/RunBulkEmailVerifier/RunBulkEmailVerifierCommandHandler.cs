using System.Text.RegularExpressions;
using MediatR;

namespace Atlas.Application.Features.MarketerScripts.Commands.RunBulkEmailVerifier;

public partial class RunBulkEmailVerifierCommandHandler : IRequestHandler<RunBulkEmailVerifierCommand, BulkEmailVerifyResult>
{
    public Task<BulkEmailVerifyResult> Handle(RunBulkEmailVerifierCommand request, CancellationToken cancellationToken)
    {
        var invalid = new List<string>();
        var valid = 0;

        foreach (var email in request.Emails)
        {
            if (EmailRegex().IsMatch(email.Trim()))
                valid++;
            else
                invalid.Add(email);
        }

        return Task.FromResult(new BulkEmailVerifyResult(request.Emails.Count, valid, invalid.Count, invalid));
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
}

