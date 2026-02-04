using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;

namespace Atlas.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
    : IRequestHandler<CreateProjectCommand, Guid>
{
    public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = ProjectProfile.Create(
            request.Name,
            request.Type,
            request.RootPath,
            request.StartupPath,
            request.MigrationPath,
            Guid.Parse(currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated."))
        );

        await applicationDbContext.ProjectProfiles.AddAsync(project, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return project.Id;
    }
}