using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Entities.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Persistence.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<AppUser,IdentityRole<Guid>,Guid>(options), IApplicationDbContext
{
    public DbSet<AppUser> Users { get; set; } 
    public DbSet<Persona> Personas { get; set; } 
    public DbSet<Integration> Integrations { get; set; } 
    public DbSet<Workspace> Workspaces { get; set; } = null!;
    public DbSet<WorkspaceIntegration> WorkspaceIntegrations { get; set; } = null!;
    public DbSet<Profession> Professions { get; set; } 
    public DbSet<Interest> Interests { get; set; }
    public DbSet<OnboardingQuestion> OnboardingQuestions { get; set; }
    public DbSet<OnboardingOption> OnboardingOptions { get; set; }
    public DbSet<UserActivity> UserActivities { get; set; }
    public DbSet<Snippet> Snippets { get; set; }
    public DbSet<Script> Scripts { get; set; }
    public DbSet<FocusSession> FocusSessions { get; set; }
    public DbSet<ProjectProfile> ProjectProfiles { get; set; }
    public DbSet<DesignAsset> DesignAssets { get; set; }
    public DbSet<DesignPalette> DesignPalettes { get; set; }
    public DbSet<PaletteColor> PaletteColors { get; set; }

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

        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles", "identity");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles", "identity");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims", "identity");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins", "identity");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims", "identity");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens", "identity");
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
                    if (entry.Entity.CreatedAt == default)
                        entry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = now;
                    

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