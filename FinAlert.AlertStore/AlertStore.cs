using FinAlert.AlertStore.Exceptions;
using FinAlert.Common.Contracts;
using FinAlert.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinAlert.AlertStore;

public class AlertStore : DbContext, IAlertStore
{
    private readonly ILogger<AlertStore> _logger;
    
    protected DbSet<PriceAlert> Alerts { get; set; }

    public AlertStore(DbContextOptions<AlertStore> options, ILogger<AlertStore> logger) : base(options)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<PriceAlert>> GetAlertsAsync(Guid userId)
    {
        return await Alerts.Where(a => a.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<PriceAlert>> GetEnabledAlertsAsync(Guid userId)
    {
        return await Alerts.Where(a => a.UserId == userId && a.Enabled).ToListAsync();
    }

    public async Task<PriceAlert?> GetAlertAsync(Guid id)
    {
        return await Alerts.FindAsync(id);
    }

    public async Task AddAlertAsync(PriceAlert alert)
    {
        ArgumentNullException.ThrowIfNull(alert);

        Alerts.Add(alert);
        await SaveChangesAsync();
    }

    public async Task UpdateAlertAsync(PriceAlert alert)
    {
        // TODO: better set only changed properties (as input parameters).
        ArgumentNullException.ThrowIfNull(alert);

        if(!await Alerts.AnyAsync(a => a.Id == alert.Id))
            throw new AlertNotFoundException($"Alert with id {alert.Id} not found");
        
        alert.UpdatedAt = DateTime.UtcNow;

        Alerts.Update(alert);
        await SaveChangesAsync();
    }

    public async Task TriggerAlertAsync(Guid id)
    {
        var alert = Alerts.Find(id);
        if(alert is null)
            throw new AlertNotFoundException($"Alert with id {id} not found");
        
        if(alert.Triggered)
        {
            _logger.LogInformation("Alert with id {AlertId} already triggered", id);

            return;
        }

        if(!alert.Enabled)
            throw new InvalidOperationException($"Alert {id} is disabled");

        alert.Enabled = false;
        alert.Triggered = true;
        alert.TriggeredAt = DateTime.UtcNow;

        await SaveChangesAsync();
    }

    public async Task EnableAlertAsync(Guid id)
    {
        var alert = Alerts.Find(id);
        if(alert is null)
            throw new AlertNotFoundException($"Alert with id {id} not found");
                
        if(alert.Enabled)
            return;

        if(alert.Triggered)
            _logger.LogInformation("Activating alert with id {AlertId}, previously triggered at {AlertTriggeredAt}", id, alert.TriggeredAt);

        alert.Enabled = true;
        alert.Triggered = false;
        alert.UpdatedAt = DateTime.UtcNow;
        alert.TriggeredAt = null;

        await SaveChangesAsync();
    }

    public async Task DisableAlertAsync(Guid id)
    {
        var alert = Alerts.Find(id);
        if(alert is null)
            throw new AlertNotFoundException($"Alert with id {id} not found");
        
        if(!alert.Enabled)
            return;

        alert.Enabled = false;
        alert.UpdatedAt = DateTime.UtcNow;

        await SaveChangesAsync();
    }

    public async Task DeleteAlertAsync(Guid id)
    {
        var alert = await Alerts.FindAsync(id);
        if(alert is null)
        {
            _logger.LogInformation("Alert with id {Id} already deleted", id);

            return;
        }
        
        Alerts.Remove(alert);
        await SaveChangesAsync();
    }
}