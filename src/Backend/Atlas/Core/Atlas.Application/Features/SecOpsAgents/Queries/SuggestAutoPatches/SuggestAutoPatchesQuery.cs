using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsAgents.Queries.SuggestAutoPatches;

public record SuggestAutoPatchesQuery(string ProjectPath) : IRequest<List<PatchSuggestion>>;

