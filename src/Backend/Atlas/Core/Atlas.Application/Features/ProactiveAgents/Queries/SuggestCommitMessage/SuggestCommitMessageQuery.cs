using MediatR;

namespace Atlas.Application.Features.ProactiveAgents.Queries.SuggestCommitMessage;

public record SuggestCommitMessageQuery(string DiffContent) : IRequest<string>;

