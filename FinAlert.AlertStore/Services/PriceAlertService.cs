using FinAlert.AlertStore.Core.Contracts;
using FinAlert.AlertStore.Core.Domain;
using Microsoft.Extensions.Logging;

namespace FinAlert.AlertStore.Services;

public class PriceAlertService : IPriceAlertService
{
    private readonly IAlertRepository _alertRepository;
    private readonly ILogger<PriceAlertService> _logger;

    internal PriceAlertService(IAlertRepository alertRepository, ILogger<PriceAlertService> logger)
    {
        _alertRepository = alertRepository;
        _logger = logger;
    }

    public Task CreateAlertAsync(PriceAlert alert)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAlertAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DisableAlertAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task EnableAlertAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<PriceAlert?> GetAlertAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PriceAlert>> GetAlertsAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PriceAlert>> GetEnabledAlertsAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task TriggerAlertAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAlertAsync(Guid alertId, PriceAlertType alertType, PriceTriggerType triggerType, decimal threshold)
    {
        throw new NotImplementedException();
    }
}