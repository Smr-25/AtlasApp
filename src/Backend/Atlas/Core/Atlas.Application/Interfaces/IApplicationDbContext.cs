using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<AppUser> Users { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}