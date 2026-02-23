using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.TeamInfo.Commands.ManageVaultLink;

public class AddVaultLinkCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<AddVaultLinkCommand, Guid>
{
    public async Task<Guid> Handle(AddVaultLinkCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var isMember = await dbContext.TeamMembers
            .AnyAsync(m => m.TeamId == request.TeamId && m.UserId == userId && !m.IsDeleted, cancellationToken);

        if (!isMember)
            throw new ForbiddenException("You are not a member of this team.");

        var link = TeamVaultLink.Create(request.TeamId, request.Label, request.Url, request.Icon, request.SortOrder);
        await dbContext.TeamVaultLinks.AddAsync(link, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return link.Id;
    }
}

public class UpdateVaultLinkCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdateVaultLinkCommand, Unit>
{
    public async Task<Unit> Handle(UpdateVaultLinkCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var isMember = await dbContext.TeamMembers
            .AnyAsync(m => m.TeamId == request.TeamId && m.UserId == userId && !m.IsDeleted, cancellationToken);

        if (!isMember)
            throw new ForbiddenException("You are not a member of this team.");

        var link = await dbContext.TeamVaultLinks
            .FirstOrDefaultAsync(v => v.Id == request.LinkId && v.TeamId == request.TeamId && !v.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("VaultLink", request.LinkId);

        link.Update(request.Label, request.Url, request.Icon, request.SortOrder);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

public class DeleteVaultLinkCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<DeleteVaultLinkCommand, Unit>
{
    public async Task<Unit> Handle(DeleteVaultLinkCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var isMember = await dbContext.TeamMembers
            .AnyAsync(m => m.TeamId == request.TeamId && m.UserId == userId && !m.IsDeleted, cancellationToken);

        if (!isMember)
            throw new ForbiddenException("You are not a member of this team.");

        var link = await dbContext.TeamVaultLinks
            .FirstOrDefaultAsync(v => v.Id == request.LinkId && v.TeamId == request.TeamId && !v.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("VaultLink", request.LinkId);

        link.Delete();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

