using MediatR;

namespace Atlas.Application.Features.LeaderScripts.Commands.RunReleaseNoteGen;

public record RunReleaseNoteGenCommand(string RepoName, string FromTag, string ToTag) : IRequest<string>;

