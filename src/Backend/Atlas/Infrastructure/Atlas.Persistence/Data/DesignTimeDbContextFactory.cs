using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Atlas.Persistence.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        
        if (basePath.Contains("Atlas.Persistence"))
            basePath = Path.GetFullPath(Path.Combine(basePath, "../../Presentation/Atlas.WebAPI"));
        else if (!basePath.Contains("Atlas.WebAPI"))
        {
            var webApiPath = Path.Combine(basePath, "Presentation/Atlas.WebAPI");
            if (Directory.Exists(webApiPath))
                basePath = webApiPath;
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile("appsettings.Mac.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string 'PostgreSqlConnection' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        optionsBuilder.UseNpgsql(
            connectionString,
            npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "atlas"));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
