using Atlas.Application.Common.Models;
using Atlas.Application.Features.Reflections.Dtos;
using MediatR;

namespace Atlas.Application.Features.Reflections.Commands.UpdateReflection;

public record UpdateReflectionCommand(
    Guid Id,
    string? Content,
    int? MoodScore,
    List<string>? Tags
) : IRequest<ResponseModel<ReflectionDto>>;