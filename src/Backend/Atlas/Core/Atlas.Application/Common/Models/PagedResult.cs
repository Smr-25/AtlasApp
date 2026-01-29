namespace Atlas.Application.Common.Models;

public record PagedResult(
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    DateTime DateFrom,
    DateTime DateTo
);