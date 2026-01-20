using Atlas.Application.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Persistance.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Persistance;

public static class ServiceRegistration
{
    extension(IServiceCollection services)
    {
        public void AddPersistanceServices(IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddIdentityCore<AppUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        }
    }
}