namespace Atlas.Application.Features.Docker.Dtos;

public record ContainerDto(
    string Id,
    string Name,
    string Image,
    string State,
    string Status,
    string PortMapping
);