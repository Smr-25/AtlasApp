using Atlas.Application.MappingProfiles;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Application;

public static class ServiceRegistration
{
    extension(IServiceCollection services){
        public void AddApplicationServices()
        {
            services.AddAutoMapper(opt=> opt.AddProfile<MapProfile>());
            services.AddValidatorsFromAssembly(typeof(ServiceRegistration).Assembly);
        }
    }
}