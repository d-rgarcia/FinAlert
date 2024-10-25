using FinAlert.Common.Enums;

namespace FinAlert.Common.Models
{
    public class PriceAlert
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public decimal Percent { get; set; }
        public AlertType AlertType { get; set; }
        public string UserId { get; set; }
    }
}