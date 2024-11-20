using FinAlert.AlertStore.Core.Domain;

namespace FinAlert.StockAlertApi.Models;
public class CreatePriceAlertRequest
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Threshold { get; set; }
    public PriceAlertType AlertType { get; set; }
    public PriceTriggerType TriggerType { get; set; }
    public bool Enabled { get; set; }
}