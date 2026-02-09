using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Docker.Queries.GetContainerLogs;

public class GetContainerLogsQueryHandler(IDockerAdapter dockerAdapter) 
    : IRequestHandler<GetContainerLogsQuery, string>
{
    public async Task<string> Handle(GetContainerLogsQuery request, CancellationToken ct)
    {
        return await dockerAdapter.GetContainerLogsAsync(request.ContainerId, request.TailCount, ct);
    }
}