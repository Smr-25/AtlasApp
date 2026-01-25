using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<Persona> Personas { get; set; }
    public DbSet<PersonaState> PersonaStates { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Constraint> Constraints { get; set; }
    Task<int> SaveChangesAsync();
}