using Atlas.Application.Common.Models;
using Atlas.Application.Features.Reflections.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Reflections.Queries.GetReflectionsByType;

public record GetReflectionsByTypeQuery(
    ReflectionType? Type,
    int? PageNumber,
    int? PageSize
) : IRequest<ResponseModel<PagedResult>>;