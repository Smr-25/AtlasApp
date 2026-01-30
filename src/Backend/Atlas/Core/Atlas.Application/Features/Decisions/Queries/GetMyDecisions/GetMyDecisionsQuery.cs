using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Dtos;
using Atlas.Domain.Enums;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace Atlas.Application.Features.Decisions.Queries.GetMyDecisions;

public record GetMyDecisionsQuery(
    int? PageNumber,
    int PageSize,
    DecisionStatus? Status
) : IRequest<ResponseModel<PagedResult>>;
