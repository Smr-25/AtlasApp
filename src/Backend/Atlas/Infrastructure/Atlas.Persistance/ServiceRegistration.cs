using System.Text;
using Atlas.Application.Interfaces;
using Atlas.Application.Settings;
using Atlas.Domain.Entities;
using Atlas.Persistance.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            
            var passwordPolicy = configuration.GetSection("PasswordPolicySettings").Get<PasswordPolicySettings>() 
                                 ?? new PasswordPolicySettings();
            
            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequiredLength = passwordPolicy.RequiredLength;
                opt.Password.RequireDigit = passwordPolicy.RequireDigit;
                opt.Password.RequireLowercase = passwordPolicy.RequireLowercase;
                opt.Password.RequireUppercase = passwordPolicy.RequireUppercase;
                opt.Password.RequireNonAlphanumeric = passwordPolicy.RequireNonAlphanumeric;
                opt.Password.RequiredUniqueChars = passwordPolicy.RequiredUniqueChars;
                
                opt.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                opt.User.RequireUniqueEmail = true;
                opt.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            
            services.Configure<PasswordHasherOptions>(options =>
            {
                options.IterationCount = 100000; 
            });
            
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() 
                              ?? new JwtSettings();
            
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });
        }
    }
}