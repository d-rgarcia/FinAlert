using FinAlert.AlertStore.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinAlert.AlertStore.Infrastructure;

public class AlertDbContext : DbContext
{
    private readonly ILogger<AlertDbContext> _logger;

    internal DbSet<PriceAlert> PriceAlerts { get; set; }

    public AlertDbContext(DbContextOptions<AlertDbContext> options, ILogger<AlertDbContext> logger) : base(options)
    {
        _logger = logger;
    }
}