using Atlas.Application.Interfaces;
using Atlas.Application.MappingProfiles;
using Atlas.Application.Services.Concretes;
using Atlas.Application.Services.Interfaces;
using Atlas.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Application;

public static class ServiceRegistration
{
    extension(IServiceCollection services)
    {
        public void AddApplicationServices()
        {
            services.AddAutoMapper(opt => opt.AddProfile<MapProfile>());
            services.AddValidatorsFromAssembly(typeof(ServiceRegistration).Assembly);
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IJwtService,JwtService>();
            
        }
    }
}