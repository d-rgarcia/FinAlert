using FinAlert.Identity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class IndentityContextFactory : IDesignTimeDbContextFactory<IndentityContext>
{
    public IndentityContext CreateDbContext(string[] args)
    {
        var aspnetEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{aspnetEnvironment}.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<IndentityContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("IdentityDb"));

        var logger = new LoggerFactory();
        
        optionsBuilder.UseLoggerFactory(logger);
        
        return new IndentityContext(optionsBuilder.Options);
    }
}