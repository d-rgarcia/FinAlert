namespace FinAlert.AlertStore.Core.Domain
{
    public class PriceAlert
    {
        public PriceAlert()
        {
            Id = Guid.NewGuid();
            CreatedAt = UpdatedAt = System.DateTime.UtcNow;
        }

        public Guid Id { get; init; }
        public Guid UserId { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public decimal Threshold { get; set; }
        public PriceAlertType AlertType { get; set; }
        public PriceTriggerType TriggerType { get; set; }
        public bool Triggered { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? TriggeredAt { get; set; }
    }
}