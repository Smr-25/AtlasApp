using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderUtilities.Queries.ConvertTimezones;

public record ConvertTimezonesQuery(List<TeamMemberTimezone> Members) : IRequest<TimezoneConversionResult>;

