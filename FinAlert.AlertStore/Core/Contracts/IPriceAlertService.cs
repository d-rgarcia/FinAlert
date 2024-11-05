using FinAlert.AlertStore.Core.Domain;

namespace FinAlert.AlertStore.Core.Contracts;

public interface IPriceAlertService
{
    Task CreateAlertAsync(PriceAlert alert);
    Task DeleteAlertAsync(Guid id);
    Task<PriceAlert?> GetAlertAsync(Guid id);
    Task<IEnumerable<PriceAlert>> GetAlertsAsync(Guid userId);
    Task<IEnumerable<PriceAlert>> GetEnabledAlertsAsync(Guid userId);
    Task EnableAlertAsync(Guid id);
    Task DisableAlertAsync(Guid id);
    Task TriggerAlertAsync(Guid id);
    Task UpdateAlertAsync(Guid alertId, PriceAlertType alertType, PriceTriggerType triggerType, decimal threshold);
}