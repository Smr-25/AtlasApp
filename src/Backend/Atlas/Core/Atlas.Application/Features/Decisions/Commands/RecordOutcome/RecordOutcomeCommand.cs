using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Decisions.Commands.RecordOutcome;

public record RecordOutcomeCommand(
    Guid DecisionId,
    OutcomeStatus Status,
    string? Description,
    bool WasExpected,
    string? LessonLearned
) : IRequest<ResponseModel<DecisionOutcomeDto>>;