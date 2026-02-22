using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.FlushCache;

public record FlushCacheCommand(string? RedisConnectionString, bool FlushMemory = true) : IRequest<string>;

