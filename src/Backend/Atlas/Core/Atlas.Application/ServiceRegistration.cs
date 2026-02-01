using System.Reflection;
using Atlas.Application.Common.Behaviors;
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
            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
            
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(opt => {
                opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                opt.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));
                opt.AddOpenBehavior(typeof(LoggingBehavior<,>));
                opt.AddOpenBehavior(typeof(PerformanceBehavior<,>));
                opt.AddOpenBehavior(typeof(ValidationBehavior<,>));
                
            });
            
            services.Configure<LockoutSettings>(configuration.GetSection("LockoutSettings"));
        }
    }
}