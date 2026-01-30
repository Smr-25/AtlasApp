using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Reflections.Commands.DeleteReflection;

public class DeleteReflectionCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<DeleteReflectionCommand, ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(DeleteReflectionCommand request, CancellationToken cancellationToken)
    {
        var reflection = await applicationDbContext.Reflections
            .FirstOrDefaultAsync(r => r.Id == request.ReflectionId, cancellationToken);

        if (reflection == null)
            throw new NotFoundException("Reflection", request.ReflectionId);

        applicationDbContext.Reflections.Remove(reflection);
        await applicationDbContext.SaveChangesAsync();

        return ResponseModel<bool>.Success(true);
    }
}