using MediatR;

namespace Atlas.Application.Features.DesignUtilities.Queries.GenerateDummyData;

public record GenerateDummyDataQuery(string Type, int Count = 10) : IRequest<List<Dictionary<string, string>>>;

