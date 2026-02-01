using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Common.Interfaces;

/// <summary>
/// Abstraction for the application database context.
/// Provides access to all DbSets and save functionality.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// User personas (Developer, Designer, etc.)
    /// </summary>
    DbSet<Persona> Personas { get; }
    
    /// <summary>
    /// External tool integrations (GitHub, Figma, etc.)
    /// </summary>
    DbSet<Integration> Integrations { get; }
    
    /// <summary>
    /// Logical workspaces for organizing work
    /// </summary>
    DbSet<Workspace> Workspaces { get; }
    
    /// <summary>
    /// Join table for workspace-integration relationships
    /// </summary>
    DbSet<WorkspaceIntegration> WorkspaceIntegrations { get; }
    
    /// <summary>
    /// Saves all changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of affected rows.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}