using FinAlert.AlertStore.Core.Domain;

namespace FinAlert.AlertStore.Core.Contracts;

public interface IAlertRepository
{
    #region UserPriceAlerts
    Task AddPriceAlertAsync(PriceAlert alert);
    Task DeletePriceAlertAsync(Guid userId, Guid id);
    Task<PriceAlert?> GetPriceAlertAsync(Guid userId, Guid id);
    Task<IEnumerable<PriceAlert>> GetPriceAlertsAsync(Guid userId);
    Task<IEnumerable<PriceAlert>> GetEnabledPriceAlertsAsync(Guid userId);
    Task EnablePriceAlertAsync(Guid userId, Guid id);
    Task DisablePriceAlertAsync(Guid userId, Guid id);
    Task TriggerPriceAlertAsync(Guid userId, Guid id);
    Task UpdatePriceAlertAsync(Guid userId, Guid alertId, PriceAlertType alertType, PriceTriggerType triggerType, decimal threshold);
    #endregion

    #region PriceAlerts
    // This section is for the PriceAlerts that are not associated with a user and should be triggered by a background service

    #endregion
}