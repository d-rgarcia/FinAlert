using FinAlert.AlertStore.Core.Domain;

namespace FinAlert.StockAlertApi.Models;
public class UpdatePriceAlertRequest
{
    public decimal Threshold { get; set; }
    public PriceAlertType AlertType { get; set; }
    public PriceTriggerType TriggerType { get; set; }
    public bool Enabled { get; set; }
}