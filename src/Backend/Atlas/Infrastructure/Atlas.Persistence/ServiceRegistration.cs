using System.Text;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Settings;
using Atlas.Domain.Entities;
using Atlas.Persistence.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Atlas.Persistence;

public static class ServiceRegistration
{
    extension(IServiceCollection services)
    {
        public void AddPersistanceServices(IConfiguration configuration)
        {
            // Configure PostgreSQL with Npgsql
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "atlas");
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorCodesToAdd: null);
                    });
                
                // Enable sensitive data logging in development
                #if DEBUG
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                #endif
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

            services.Configure<PasswordHasherOptions>(options => { options.IterationCount = 100000; });

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