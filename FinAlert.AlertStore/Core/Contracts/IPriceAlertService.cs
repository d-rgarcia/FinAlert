using FinAlert.AlertStore.Core.Domain;

namespace FinAlert.AlertStore.Core.Contracts;

public interface IPriceAlertService
{
    Task CreateAlertAsync(PriceAlert alert);
    Task DeleteAlertAsync(Guid userId, Guid alertId);
    Task<PriceAlert?> GetAlertAsync(Guid userId, Guid alertId);
    Task<IEnumerable<PriceAlert>> GetAlertsAsync(Guid userId);
    Task<IEnumerable<PriceAlert>> GetEnabledAlertsAsync(Guid userId);
    Task EnableAlertAsync(Guid userId, Guid alertId);
    Task DisableAlertAsync(Guid userId, Guid alertId);
    Task TriggerAlertAsync(Guid userId, Guid alertId);
    Task UpdateAlertAsync(Guid userId, Guid alertId, PriceAlertType alertType, PriceTriggerType triggerType, decimal threshold);
}