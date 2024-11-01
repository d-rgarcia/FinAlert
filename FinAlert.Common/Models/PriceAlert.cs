using FinAlert.Common.Enums;

namespace FinAlert.Common.Models
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
        public AlertType AlertType { get; set; }
        public TriggerType TriggerType { get; set; }
        public bool Triggered { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? TriggeredAt { get; set; }
    }
}