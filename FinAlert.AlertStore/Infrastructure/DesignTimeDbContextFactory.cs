using FinAlert.AlertStore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;

/// <summary>
/// Used for EF CLI tooling to create migrations
/// </summary>
public class AlertDbContextFactory : IDesignTimeDbContextFactory<AlertDbContext>
{
    public AlertDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AlertDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=StockAlert;Username=postgres;Password=Admin123");

        var logger = new LoggerFactory().CreateLogger<AlertDbContext>();

        return new AlertDbContext(optionsBuilder.Options, logger);
    }
}