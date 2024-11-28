namespace FinAlert.StockQueryService.Core.Domain;

public class StockPrice
{
    public string Currency { get; set; } = string.Empty;

    public decimal Current;

    public decimal DayHigh { get; set; }

    public decimal DayLow { get; set; }

    public decimal Open { get; set; }

    public decimal PreviousClose { get; set; }

    public DateTime MarketTime { get; set; } = new DateTime();
}