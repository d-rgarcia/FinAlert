using FinAlert.AlertStore.Core.Contracts;
using FinAlert.AlertStore.Core.Domain;
using Microsoft.Extensions.Logging;

namespace FinAlert.AlertStore.Services;

public class PriceAlertService : IPriceAlertService
{
    private readonly IAlertRepository _alertRepository;
    private readonly ILogger<PriceAlertService> _logger;

    public PriceAlertService(IAlertRepository alertRepository, ILogger<PriceAlertService> logger)
    {
        _alertRepository = alertRepository;
        _logger = logger;
    }

    public async Task CreateAlertAsync(PriceAlert alert)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(alert);

            if (alert.Triggered)
                throw new ArgumentException("Alert cannot be created in triggered state");
            if (alert.Threshold <= 0)
                throw new ArgumentException("Alert threshold must be greater than 0");
            if (string.IsNullOrEmpty(alert.Symbol))
                throw new ArgumentException("Alert must have a symbol");

            await _alertRepository.AddPriceAlertAsync(alert);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating alert");

            throw;
        }
    }

    public async Task DeleteAlertAsync(Guid userId, Guid alertId)
    {
        try
        {
            await _alertRepository.DeletePriceAlertAsync(userId, alertId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting alert");

            throw;
        }
    }

    public async Task DisableAlertAsync(Guid userId, Guid alertId)
    {
        try
        {
            await _alertRepository.DisablePriceAlertAsync(userId, alertId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disabling alert");

            throw;
        }
    }

    public async Task EnableAlertAsync(Guid userId, Guid alertId)
    {
        try
        {
            await _alertRepository.EnablePriceAlertAsync(userId, alertId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enabling alert");

            throw;
        }
    }

    public async Task<PriceAlert?> GetAlertAsync(Guid userId, Guid id)
    {
        try
        {
            return await _alertRepository.GetPriceAlertAsync(userId, id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting alert");

            throw;
        }
    }

    public async Task<IEnumerable<PriceAlert>> GetAlertsAsync(Guid userId)
    {
        try
        {
            return await _alertRepository.GetPriceAlertsAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting alerts");

            throw;
        }
    }

    public async Task<IEnumerable<PriceAlert>> GetEnabledAlertsAsync(Guid userId)
    {
        try
        {
            return await _alertRepository.GetEnabledPriceAlertsAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting enabled alerts");

            throw;
        }
    }

    public async Task TriggerAlertAsync(Guid userId, Guid alertId)
    {
        try
        {
            await _alertRepository.TriggerPriceAlertAsync(userId, alertId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering alert");

            throw;
        }
    }

    public async Task UpdateAlertAsync(Guid userId, Guid alertId, PriceAlertType alertType, PriceTriggerType triggerType, decimal threshold)
    {
        try
        {
            if (threshold <= 0)
                throw new ArgumentException("Alert threshold must be greater than 0");

            await _alertRepository.UpdatePriceAlertAsync(userId, alertId, alertType, triggerType, threshold);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating alert");

            throw;
        }
    }
}