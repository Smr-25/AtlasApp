using Docker.DotNet;
using Docker.DotNet.Models;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Docker.Dtos;
using Atlas.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Adapters;

public class DockerAdapter : IDockerAdapter
{
    private readonly DockerClient _client;
    private readonly ILogger<DockerAdapter> _logger;

    public DockerAdapter(ILogger<DockerAdapter> logger)
    {
        _logger = logger;
        var dockerUri =
            System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform
                .Windows)
                ? new Uri("npipe://./pipe/docker_engine")
                : new Uri("unix:///var/run/docker.sock");

        _client = new DockerClientConfiguration(dockerUri).CreateClient();
        _logger.LogInformation("Docker client initialized with URI: {DockerUri}", dockerUri);
    }


    public async Task<List<ContainerDto>> GetContainersAsync(CancellationToken ct)
    {
        try
        {
            var containers = await _client.Containers.ListContainersAsync(
                new ContainersListParameters { All = true }, ct);

            _logger.LogDebug("Retrieved {Count} containers from Docker", containers.Count);

            return containers.Select(c => new ContainerDto(
                c.ID,
                c.Names.FirstOrDefault()?.TrimStart('/') ?? "No Name",
                c.Image,
                c.State,
                c.Status,
                string.Join(", ", c.Ports.Select(p => $"{p.PublicPort}:{p.PrivatePort}")),
                c.State == "running"
            )).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get containers from Docker");
            throw;
        }
    }

    public async Task StartContainerAsync(string containerId, CancellationToken ct)
    {
        try
        {
            await _client.Containers.StartContainerAsync(containerId, new ContainerStartParameters(), ct);
            _logger.LogInformation("Container {ContainerId} started successfully", containerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start container {ContainerId}", containerId);
            throw;
        }
    }

    public async Task StopContainerAsync(string containerId, CancellationToken ct)
    {
        try
        {
            await _client.Containers.StopContainerAsync(containerId, new ContainerStopParameters(), ct);
            _logger.LogInformation("Container {ContainerId} stopped successfully", containerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop container {ContainerId}", containerId);
            throw;
        }
    }

    public async Task RestartContainerAsync(string containerId, CancellationToken ct)
    {
        try
        {
            await _client.Containers.RestartContainerAsync(containerId, new ContainerRestartParameters(), ct);
            _logger.LogInformation("Container {ContainerId} restarted successfully", containerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restart container {ContainerId}", containerId);
            throw;
        }
    }

    public async Task<string> GetContainerLogsAsync(string containerId, int tailCount, CancellationToken ct)
    {
        try
        {
            var logsStream = await _client.Containers.GetContainerLogsAsync(containerId, false, new ContainerLogsParameters
            {
                ShowStdout = true,
                ShowStderr = true,
                Tail = tailCount.ToString(),
                Timestamps = true
            }, ct);

            var streamReader = logsStream.ToString();
            using var reader = new StreamReader(streamReader!);
            var logs = await reader.ReadToEndAsync(ct);
            _logger.LogDebug("Retrieved {Length} bytes of logs for container {ContainerId}", logs.Length, containerId);
            return logs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get logs for container {ContainerId}", containerId);
            throw;
        }
    }
    
    public async Task<bool> RemoveContainerAsync(string containerId, bool force, CancellationToken ct)
    {
        try
        {
            await _client.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters
            {
                Force = force,
                RemoveVolumes = false
            }, ct);
            
            _logger.LogInformation("Container {ContainerId} removed successfully (force: {Force})", containerId, force);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove container {ContainerId}", containerId);
            throw;
        }
    }
}


