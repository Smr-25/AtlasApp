using Atlas.Application.Common.Models;
using Atlas.Application.Features.Reflections.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Reflections.Commands.CreateReflection;

public record CreateReflectionCommand(
    string Content,
    ReflectionType ReflectionType,
    Guid DecisionId,
    int? MoodScore,
    List<string?> Tags
) : IRequest<ResponseModel<ReflectionDto>>;