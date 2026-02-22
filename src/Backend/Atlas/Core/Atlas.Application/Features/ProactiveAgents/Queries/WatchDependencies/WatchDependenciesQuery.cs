using MediatR;

namespace Atlas.Application.Features.ProactiveAgents.Queries.WatchDependencies;

public record WatchDependenciesQuery(string ProjectFilePath) : IRequest<List<string>>;

