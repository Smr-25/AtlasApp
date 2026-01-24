using System.Threading.RateLimiting;
using Atlas.Application.MappingProfiles;
using Atlas.Application.Services.Concretes;
using Atlas.Application.Services.Interfaces;
using Atlas.Application.Settings;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Application;

public static class ServiceRegistration
{
    extension(IServiceCollection services)
    {
        public void AddApplicationServices(IConfiguration configuration)
        {
            services.AddAutoMapper(opt => opt.AddProfile<MapProfile>());
            services.AddValidatorsFromAssembly(typeof(ServiceRegistration).Assembly);
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IJwtService, JwtService>();
            services.Configure<LockoutSettings>(configuration.GetSection("LockoutSettings"));
        }
    }
}