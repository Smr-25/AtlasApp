using System.Reflection;
using Atlas.Application.MapProfiles;
using Atlas.Application.Settings;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Application;

public static class ServiceRegistration
{
    extension(IServiceCollection services)
    {
        public void AddApplicationServices(IConfiguration configuration)
        {
            services.AddAutoMapper(opt =>
            {
                opt.AddMaps(typeof(PersonaMapProfile).Assembly);
            });
            services.AddMediatR(opt => {
                opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            
            services.Configure<LockoutSettings>(configuration.GetSection("LockoutSettings"));
        }
    }
}