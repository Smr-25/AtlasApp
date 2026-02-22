using MediatR;

namespace Atlas.Application.Features.DesignUtilities.Queries.GenerateDummyData;

public class GenerateDummyDataQueryHandler
    : IRequestHandler<GenerateDummyDataQuery, List<Dictionary<string, string>>>
{
    public Task<List<Dictionary<string, string>>> Handle(GenerateDummyDataQuery request, CancellationToken cancellationToken)
    {
        var random = new Random();
        var result = new List<Dictionary<string, string>>();
        var firstNames = new[] { "John", "Jane", "Alex", "Maria", "Sam", "Lisa", "Omar", "Sofia", "Leo", "Emma" };
        var lastNames = new[] { "Smith", "Johnson", "Brown", "Taylor", "Wilson", "Davis", "Clark", "Lewis", "Hall", "Young" };
        var domains = new[] { "gmail.com", "yahoo.com", "outlook.com", "company.io" };

        for (int i = 0; i < request.Count; i++)
        {
            var first = firstNames[random.Next(firstNames.Length)];
            var last = lastNames[random.Next(lastNames.Length)];
            result.Add(new Dictionary<string, string>
            {
                ["name"] = $"{first} {last}",
                ["email"] = $"{first.ToLower()}.{last.ToLower()}@{domains[random.Next(domains.Length)]}",
                ["phone"] = $"+1{random.Next(200, 999)}{random.Next(1000000, 9999999)}",
                ["avatar"] = $"https://i.pravatar.cc/150?u={Guid.NewGuid()}"
            });
        }

        return Task.FromResult(result);
    }
}

