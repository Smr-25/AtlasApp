using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderInsights.Queries.GetMeetingsAvoided;

public record GetMeetingsAvoidedQuery(Guid TeamId, DateTime From, DateTime To) : IRequest<MeetingsAvoidedResult>;

