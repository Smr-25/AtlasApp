using System.Runtime.InteropServices;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Docker.Dtos;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Atlas.Infrastructure.Services;

public class DockerService : IDockerService
{
    private readonly DockerClient _client;

    public DockerService()
    {
        var dockerUri = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new Uri("npipe://./pipe/docker_engine") 
            : new Uri("unix:///var/run/docker.sock"); 

        _client = new DockerClientConfiguration(dockerUri).CreateClient();
    }

    public async Task<List<ContainerDto>> GetContainersAsync(CancellationToken cancellationToken = default)
    {
        var parameters = new ContainersListParameters { All = true };
        
        var containers = await _client.Containers.ListContainersAsync(parameters, cancellationToken);

        return containers.Select(c => new ContainerDto(
            Id: c.ID[..12], 
            Name: c.Names.FirstOrDefault()?.TrimStart('/') ?? "Unknown", 
            Image: c.Image,
            State: c.State,
            Status: c.Status,
            PortMapping: string.Join(", ", c.Ports.Select(p => $"{p.PublicPort}:{p.PrivatePort}"))
        )).ToList();
    }

    public Task StartContainerAsync(string containerId, CancellationToken cancellationToken = default)
    {
        return _client.Containers.StartContainerAsync(containerId, new ContainerStartParameters(), cancellationToken);
    }

    public Task StopContainerAsync(string containerId, CancellationToken cancellationToken = default)
    {
        return _client.Containers.StopContainerAsync(containerId, new ContainerStopParameters(), cancellationToken);
    }

    public Task RestartContainerAsync(string containerId, CancellationToken cancellationToken = default)
    {
        return _client.Containers.RestartContainerAsync(containerId, new ContainerRestartParameters(), cancellationToken);
    }
    
    public async Task<string> GetLogsAsync(string containerId, int tail = 100, CancellationToken cancellationToken = default)
    {
        var parameters = new ContainerLogsParameters
        {
            ShowStdout = true,
            ShowStderr = true,
            Tail = tail.ToString()
        };

        await using var stream = await _client.Containers.GetContainerLogsAsync(containerId, parameters, cancellationToken);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken);
    }
}