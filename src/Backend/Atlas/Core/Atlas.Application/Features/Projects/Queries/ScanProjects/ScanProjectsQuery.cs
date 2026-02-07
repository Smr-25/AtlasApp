using Atlas.Application.Features.Projects.Dtos;
using MediatR;

namespace Atlas.Application.Features.Projects.Queries.ScanProjects;

public record ScanLocalProjectsQuery(string? RootPath) : IRequest<List<LocalProjectDto>>;