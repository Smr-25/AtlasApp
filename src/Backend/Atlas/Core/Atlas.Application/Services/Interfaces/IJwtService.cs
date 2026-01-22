using Atlas.Application.Models;
using Atlas.Domain.Entities;

namespace Atlas.Application.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(AppUser user);
}