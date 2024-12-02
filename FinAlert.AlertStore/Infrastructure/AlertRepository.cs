using FinAlert.AlertStore.Core.Contracts;
using FinAlert.AlertStore.Core.Domain;
using FinAlert.AlertStore.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinAlert.AlertStore.Infrastructure;

internal class AlertRepository : IAlertRepository
{
    private readonly AlertDbContext _dbContext;
    private readonly ILogger<AlertRepository> _logger;

    public AlertRepository(ILogger<AlertRepository> logger, AlertDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PriceAlert>> GetPriceAlertsAsync(Guid userId)
    {
        return await _dbContext.PriceAlerts.Where(a => a.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<PriceAlert>> GetEnabledPriceAlertsAsync(Guid userId)
    {
        return await _dbContext.PriceAlerts.Where(a => a.UserId == userId && a.Enabled).ToListAsync();
    }

    public async Task<PriceAlert?> GetPriceAlertAsync(Guid userId, Guid alertId)
    {
        var alert = await _dbContext.PriceAlerts.FindAsync(alertId);
        if (alert?.UserId != userId)
            return null;

        return alert;
    }

    public async Task AddPriceAlertAsync(PriceAlert alert)
    {
        ArgumentNullException.ThrowIfNull(alert);

        await _dbContext.PriceAlerts.AddAsync(alert);

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePriceAlertAsync(Guid userId, Guid alertId, PriceAlertType alertType, PriceTriggerType triggerType, decimal threshold)
    {
        var existingAlert = await _dbContext.PriceAlerts.FindAsync(alertId);
        if (existingAlert is null || existingAlert.UserId != userId)
            throw new AlertNotFoundException($"Alert with id {alertId} not found");

        existingAlert.UpdatedAt = DateTime.UtcNow;
        existingAlert.Threshold = threshold;
        existingAlert.AlertType = alertType;
        existingAlert.TriggerType = triggerType;

        await _dbContext.SaveChangesAsync();
    }

    public async Task TriggerPriceAlertAsync(Guid userId, Guid alertId)
    {
        var alert = _dbContext.PriceAlerts.Find(alertId);
        if (alert is null || alert.UserId != userId)
            throw new AlertNotFoundException($"Alert with id {alertId} not found");

        if (alert.Triggered)
        {
            _logger.LogInformation("Alert with id {AlertId} already triggered", alertId);

            return;
        }

        if (!alert.Enabled)
            throw new InvalidOperationException($"Alert {alertId} is disabled");

        alert.Enabled = false;
        alert.Triggered = true;
        alert.TriggeredAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task EnablePriceAlertAsync(Guid userId, Guid alertId)
    {
        var alert = _dbContext.PriceAlerts.Find(alertId);
        if (alert is null || alert.UserId != userId)
            throw new AlertNotFoundException($"Alert with id {alertId} not found");

        if (alert.Enabled)
            return;

        if (alert.Triggered)
            _logger.LogInformation("Activating alert with id {AlertId}, previously triggered at {AlertTriggeredAt}", alertId, alert.TriggeredAt);

        alert.Enabled = true;
        alert.Triggered = false;
        alert.UpdatedAt = DateTime.UtcNow;
        alert.TriggeredAt = null;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DisablePriceAlertAsync(Guid userId, Guid alertId)
    {
        var alert = _dbContext.PriceAlerts.Find(alertId);
        if (alert is null || alert.UserId != userId)
            throw new AlertNotFoundException($"Alert with id {alertId} not found");

        if (!alert.Enabled)
            return;

        alert.Enabled = false;
        alert.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeletePriceAlertAsync(Guid userId, Guid alertId)
    {
        var alert = await _dbContext.PriceAlerts.FindAsync(alertId);
        if (alert is null || alert.UserId != userId)
        {
            _logger.LogInformation("Alert with id {Id} already deleted", alertId);

            return;
        }

        _dbContext.PriceAlerts.Remove(alert);

        await _dbContext.SaveChangesAsync();
    }

    #region PriceAlerts

    public async Task<IEnumerable<PriceAlert>> GetEnabledPriceAlertsAsync()
    {
        return await _dbContext.PriceAlerts.Where(a => a.Enabled).ToListAsync();
    }

    #endregion
}