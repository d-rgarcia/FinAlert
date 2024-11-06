using FinAlert.AlertStore.Core.Domain;

namespace FinAlert.AlertStore.Core.Contracts;

internal interface IAlertRepository
{
    Task AddPriceAlertAsync(PriceAlert alert);
    Task DeletePriceAlertAsync(Guid id);
    Task<PriceAlert?> GetPriceAlertAsync(Guid id);
    Task<IEnumerable<PriceAlert>> GetPriceAlertsAsync(Guid userId);
    Task<IEnumerable<PriceAlert>> GetEnabledPriceAlertsAsync(Guid userId);
    Task EnablePriceAlertAsync(Guid id);
    Task DisablePriceAlertAsync(Guid id);
    Task TriggerPriceAlertAsync(Guid id);
    Task UpdatePriceAlertAsync(Guid alertId, PriceAlertType alertType, PriceTriggerType triggerType, decimal threshold);
}