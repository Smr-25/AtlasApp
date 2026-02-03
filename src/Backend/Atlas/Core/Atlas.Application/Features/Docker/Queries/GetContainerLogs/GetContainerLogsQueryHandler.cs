using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Docker.Queries.GetContainerLogs;

public class GetContainerLogsQueryHandler(IDockerService dockerService) : IRequestHandler<GetContainerLogsQuery, string>
{
    public async Task<string> Handle(GetContainerLogsQuery request, CancellationToken cancellationToken)
    {
        return await dockerService.GetLogsAsync(request.ContainerId, 100, cancellationToken);
    }
}