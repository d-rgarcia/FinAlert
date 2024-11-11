using FinAlert.AlertStore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// <summary>
/// Used for EF CLI tooling to create migrations
/// </summary>
public class AlertDbContextFactory : IDesignTimeDbContextFactory<AlertDbContext>
{
    public AlertDbContext CreateDbContext(string[] args)
    {
        var aspnetEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{aspnetEnvironment}.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AlertDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("AlertsDb"));

        var logger = new LoggerFactory();
        optionsBuilder.UseLoggerFactory(logger);

        return new AlertDbContext(optionsBuilder.Options, logger.CreateLogger<AlertDbContext>());
    }
}