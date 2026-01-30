using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Reflections.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Reflections.Queries.GetReflectionById;

public class GetReflectionByIdQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    : IRequestHandler<GetReflectionByIdQuery, ResponseModel<ReflectionDto>>
{
    public async Task<ResponseModel<ReflectionDto>> Handle(GetReflectionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var reflection = await applicationDbContext.Reflections
            .FirstOrDefaultAsync(r => r.Id == request.ReflectionId, cancellationToken);

        if (reflection == null)
            throw new NotFoundException("Reflection not found");

        var reflectionDto = mapper.Map<ReflectionDto>(reflection);
        return ResponseModel<ReflectionDto>.Success(reflectionDto);
    }
}