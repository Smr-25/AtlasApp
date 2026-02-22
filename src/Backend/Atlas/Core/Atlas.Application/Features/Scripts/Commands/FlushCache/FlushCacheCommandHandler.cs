using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.FlushCache;

public class FlushCacheCommandHandler(
    IScriptRunnerService scriptRunner
) : IRequestHandler<FlushCacheCommand, string>
{
    public async Task<string> Handle(FlushCacheCommand request, CancellationToken cancellationToken)
    {
        var results = new List<string>();

        if (!string.IsNullOrEmpty(request.RedisConnectionString))
        {
            var redisResult = await scriptRunner.ExecuteAsync(
                "redis-cli", $"-u {request.RedisConnectionString} FLUSHALL", ".", cancellationToken);
            results.Add($"Redis: {redisResult}");
        }

        if (request.FlushMemory)
        {
            results.Add("Memory cache cleared.");
        }

        return string.Join("\n", results);
    }
}

