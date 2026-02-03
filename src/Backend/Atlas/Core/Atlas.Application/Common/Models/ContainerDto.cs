namespace Atlas.Application.Common.Models;

public record ContainerDto(
    string Id,
    string Name,
    string Image,
    string State,      
    string Status,     
    string PortMapping 
);