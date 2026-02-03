using Atlas.Application.Common.Models;
using Atlas.Application.Features.Docker.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface IDockerService
{
    Task<List<ContainerDto>> GetContainersAsync(CancellationToken cancellationToken = default);
    Task StartContainerAsync(string containerId, CancellationToken cancellationToken = default);
    Task StopContainerAsync(string containerId, CancellationToken cancellationToken = default);
    Task RestartContainerAsync(string containerId, CancellationToken cancellationToken = default);
    Task<string> GetLogsAsync(string containerId, int tail = 100, CancellationToken cancellationToken = default);
}