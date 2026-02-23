using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderUtilities.Queries.CalculateCapacity;

public record CalculateCapacityQuery(List<MemberCapacityInput> Members) : IRequest<CapacityResult>;

