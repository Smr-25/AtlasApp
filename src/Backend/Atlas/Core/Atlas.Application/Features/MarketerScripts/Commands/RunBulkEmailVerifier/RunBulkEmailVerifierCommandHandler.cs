using System.Text.RegularExpressions;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerScripts.Commands.RunBulkEmailVerifier;

public partial class RunBulkEmailVerifierCommandHandler(IAtlasHubService hubService, ICurrentUserService currentUser)
    : IRequestHandler<RunBulkEmailVerifierCommand, BulkEmailVerifyResult>
{
    public async Task<BulkEmailVerifyResult> Handle(RunBulkEmailVerifierCommand request, CancellationToken cancellationToken)
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

        var result = new BulkEmailVerifyResult(request.Emails.Count, valid, invalid.Count, invalid);

        var userId = currentUser.GetUserIdOrDefault();
        if (userId != null)
        {
            await hubService.SendJobCompletedAsync(userId.Value, "BulkEmailVerifier", new
            {
                Total = result.Total,
                Valid = result.Valid,
                Invalid = result.Invalid,
                InvalidEmails = result.InvalidEmails
            }, cancellationToken);
        }

        return result;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
}
