using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderScripts.Commands.RunReleaseNoteGen;

public class RunReleaseNoteGenCommandHandler(
    ILeaderScriptService scriptService,
    ICurrentUserService currentUser
) : IRequestHandler<RunReleaseNoteGenCommand, string>
{
    public async Task<string> Handle(RunReleaseNoteGenCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await scriptService.GenerateReleaseNotesAsync(userId, request.RepoName, request.FromTag, request.ToTag, cancellationToken);
    }
}

