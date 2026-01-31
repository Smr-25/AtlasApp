using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Common.Interfaces;

public interface IApplicationDbContext
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
    public DbSet<PersonaTimelineEvent> PersonaTimelineEvents { get; set; }
    Task<int> SaveChangesAsync();
}