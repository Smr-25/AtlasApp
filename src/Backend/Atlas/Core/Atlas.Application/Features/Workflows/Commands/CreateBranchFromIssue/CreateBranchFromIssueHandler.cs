using System.Text.RegularExpressions;
using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Workflows.Commands.CreateBranchFromIssue;

public class CreateBranchFromIssueHandler(
    IApplicationDbContext applicationDbContext,
    IEncryptionService encryptionService,
    IJiraAdapter jiraAdapter,
    IGitIntegrationAdapter gitAdapter
) : IRequestHandler<CreateBranchFromIssueCommand, string>
{
    public async Task<string> Handle(CreateBranchFromIssueCommand request, CancellationToken cancellationToken)
    {
        var jiraInt =
            await applicationDbContext.Integrations.FirstOrDefaultAsync(i => i.Id == request.JiraIntegrationId,
                cancellationToken);
        var gitInt =
            await applicationDbContext.Integrations.FirstOrDefaultAsync(i => i.Id == request.JiraIntegrationId,
                cancellationToken);

        if (jiraInt == null || gitInt == null) throw new NotFoundException("Integration not found");

        var jiraToken = encryptionService.Decrypt(jiraInt.EncryptedAccessToken);
        var gitToken = encryptionService.Decrypt(gitInt.EncryptedAccessToken);

        var jiraDomain = jiraInt.ApiUrl;
        var issue = await jiraAdapter.GetIssueAsync(jiraToken, jiraDomain, request.IssueKey, cancellationToken);

        var safeSummary = GenerateSlug(issue.Summary);
        var branchName = $"feature/{issue.Key.ToUpper()}-{safeSummary}";
        
        await gitAdapter.CreateBranchAsync(gitToken, request.RepoOwner, request.RepoName, request.BaseBranch,
            branchName, cancellationToken);

        await jiraAdapter.MoveIssueAsync(jiraToken, jiraDomain, request.IssueKey, "31", cancellationToken); 

        return branchName;
    }

    private string GenerateSlug(string phrase)
    {
        var str = phrase.ToLowerInvariant();
        str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); 
        str = Regex.Replace(str, @"\s+", " ").Trim(); 
        str = Regex.Replace(str, @"\s", "-");
        return str;
    }
}