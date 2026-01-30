using Atlas.Application.Common.Models;
using Atlas.Application.Features.Reflections.Dtos;
using MediatR;

namespace Atlas.Application.Features.Reflections.Queries.GetReflectionById;

public record GetReflectionByIdQuery(Guid ReflectionId) : IRequest<ResponseModel<ReflectionDto>>;