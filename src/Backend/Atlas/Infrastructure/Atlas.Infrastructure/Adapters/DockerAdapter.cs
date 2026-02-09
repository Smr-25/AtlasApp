using Docker.DotNet;
using Docker.DotNet.Models;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Docker.Dtos;

namespace Atlas.Infrastructure.Adapters;

public class DockerAdapter : IDockerAdapter
{
    private readonly DockerClient _client;

    public DockerAdapter()
    {
        var dockerUri =
            System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform
                .Windows)
                ? new Uri("npipe://./pipe/docker_engine")
                : new Uri("unix:///var/run/docker.sock");

        _client = new DockerClientConfiguration(dockerUri).CreateClient();
    }

    public async Task<List<ContainerDto>> GetContainersAsync(CancellationToken ct)
    {
        var containers = await _client.Containers.ListContainersAsync(
            new ContainersListParameters { All = true }, ct);

        return containers.Select(c => new ContainerDto
        (
            c.ID, 
            c.Names.FirstOrDefault()?.TrimStart('/') ?? "No Name",
            c.Image,
            c.State,
            c.Status,
            string.Join(", ", c.Ports.Select(p => $"{p.PublicPort}:{p.PrivatePort}")),
            c.State == "running"
        )).ToList();
    }

    public async Task StartContainerAsync(string containerId, CancellationToken ct)
    {
        await _client.Containers.StartContainerAsync(containerId, new ContainerStartParameters(), ct);
    }

    public async Task StopContainerAsync(string containerId, CancellationToken ct)
    {
        await _client.Containers.StopContainerAsync(containerId, new ContainerStopParameters(), ct);
    }

    public async Task RestartContainerAsync(string containerId, CancellationToken ct)
    {
        await _client.Containers.RestartContainerAsync(containerId, new ContainerRestartParameters(), ct);
    }

    public async Task<string> GetContainerLogsAsync(string containerId, int tailCount, CancellationToken ct)
    {
        var stream = await _client.Containers.GetContainerLogsAsync(containerId, false, new ContainerLogsParameters
        {
            ShowStdout = true,
            ShowStderr = true,
            Tail = tailCount.ToString()
        }, ct);

        var stringStream = stream.ToString();
        using var reader = new StreamReader(stringStream!);
        return await reader.ReadToEndAsync(ct);
    }
}