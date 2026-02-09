using Atlas.Application.Features.Docker.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface IDockerAdapter
{
    Task<List<ContainerDto>> GetContainersAsync(CancellationToken ct);
    Task StartContainerAsync(string containerId, CancellationToken ct);
    Task StopContainerAsync(string containerId, CancellationToken ct);
    Task RestartContainerAsync(string containerId, CancellationToken ct);
    Task<string> GetContainerLogsAsync(string containerId, int tailCount, CancellationToken ct);
}