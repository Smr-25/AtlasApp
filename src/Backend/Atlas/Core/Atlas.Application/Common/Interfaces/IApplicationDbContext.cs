using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<AppUser> Users { get; set; }
    Task<int> SaveChangesAsync();
}