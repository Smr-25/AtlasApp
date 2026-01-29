using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Atlas.Persistence.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<AppUser>(options), IApplicationDbContext
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<Persona> Personas { get; set; }
    public DbSet<PersonaState> PersonaStates { get; set; }
    public DbSet<PersonaStateHistory> PersonaStateHistories { get; set; }
    public DbSet<Decision> Decisions { get; set; }
    public DbSet<DecisionContext> DecisionContexts { get; set; }
    public DbSet<DecisionOutcome> DecisionOutcomes { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Constraint> Constraints { get; set; }
    public DbSet<Reflection> Reflections { get; set; }
    public DbSet<TimeLine> TimeLines { get; set; }
    public DbSet<PersonaTimelineEvent> PersonaTimelineEvents { get; set; }
    

    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}