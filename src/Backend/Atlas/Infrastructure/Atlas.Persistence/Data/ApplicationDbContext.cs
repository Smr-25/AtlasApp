using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Entities.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Persistence.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<AppUser>(options), IApplicationDbContext
{
    public DbSet<Persona> Personas { get; set; } = null!;
    public DbSet<Integration> Integrations { get; set; } = null!;
    public DbSet<Workspace> Workspaces { get; set; } = null!;
    public DbSet<WorkspaceIntegration> WorkspaceIntegrations { get; set; } = null!;

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        ConfigureIdentitySchema(modelBuilder);
    }

    private static void ConfigureIdentitySchema(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>().ToTable("Users", "identity");

        // Configure other Identity tables
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>()
            .ToTable("Roles", "identity");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>()
            .ToTable("UserRoles", "identity");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>()
            .ToTable("UserClaims", "identity");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>()
            .ToTable("UserLogins", "identity");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>()
            .ToTable("UserTokens", "identity");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>()
            .ToTable("RoleClaims", "identity");
    }
    
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // CreatedAt is set in the entity constructor, but ensure it's set
                    if (entry.Entity.CreatedAt == default)
                    {
                        entry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = now;
                    }

                    break;

                case EntityState.Modified:
                    entry.Property(nameof(BaseEntity.ModifiedAt)).CurrentValue = now;
                    entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}