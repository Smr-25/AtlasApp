using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Reflections.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Reflections.Commands.UpdateReflection;

public class UpdateReflectionCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    : IRequestHandler<UpdateReflectionCommand, ResponseModel<ReflectionDto>>
{
    public async Task<ResponseModel<ReflectionDto>> Handle(UpdateReflectionCommand request,
        CancellationToken cancellationToken)
    {
        var reflection = await applicationDbContext.Reflections.FirstOrDefaultAsync(
            x => x.Id == request.Id, cancellationToken);
        if (reflection == null)
            throw new NotFoundException("Reflection not found");
        if (request.Content != null)
            reflection.UpdateContent(request.Content);
        if (request.MoodScore.HasValue)
            reflection.SetMoodScore(request.MoodScore.Value);
        if (request.Tags != null)
            foreach (var tag in request.Tags)
                reflection.AddTag(tag);
        applicationDbContext.Reflections.Update(reflection);
        await applicationDbContext.SaveChangesAsync();
        var reflectionDto = mapper.Map<ReflectionDto>(reflection);
        return ResponseModel<ReflectionDto>.Success(reflectionDto);
    }
}