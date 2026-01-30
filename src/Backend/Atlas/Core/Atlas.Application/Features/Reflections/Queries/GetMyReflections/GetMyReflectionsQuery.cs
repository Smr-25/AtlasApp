using Atlas.Application.Common.Models;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Reflections.Queries.GetMyReflections;

public record GetReflectionsQuery(
    int? PageNumber,
    int? PageSize,
    ReflectionType? Type,
    DateTime? DateFrom,
    DateTime? DateTo
) : IRequest<ResponseModel<PagedResult>>;
