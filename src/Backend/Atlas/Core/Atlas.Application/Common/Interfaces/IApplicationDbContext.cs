using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Persona> Personas { get; }
    DbSet<Integration> Integrations { get; }
    DbSet<Workspace> Workspaces { get; }
    DbSet<WorkspaceIntegration> WorkspaceIntegrations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}