using FinAlert.Common.Models;

namespace FinAlert.Common.Contracts
{
    public interface IAlertStore
    {
        Task<IEnumerable<PriceAlert>> GetAlertsAsync(Guid userId);
        Task TriggerAlertAsync(Guid id);
        Task EnableAlertAsync(Guid id);
        Task DisableAlertAsync(Guid id);
        Task<PriceAlert?> GetAlertAsync(Guid id);
        Task AddAlertAsync(PriceAlert alert);
        Task UpdateAlertAsync(PriceAlert alert);
        Task DeleteAlertAsync(Guid id);
    }
}