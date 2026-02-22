using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.ResolveGitConflicts;

public record ResolveGitConflictsCommand(string RepositoryPath, string Strategy = "theirs") : IRequest<string>;

