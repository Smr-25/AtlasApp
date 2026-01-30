using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Reflections.Commands.DeleteReflection;

public record DeleteReflectionCommand(
    Guid ReflectionId
) : IRequest<ResponseModel<bool>>;