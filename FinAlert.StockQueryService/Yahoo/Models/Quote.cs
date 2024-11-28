using System.Text.Json.Serialization;

namespace FinAlert.StockQueryService.Core.Models
{
    public class QuoteRoot
    {
        [JsonPropertyName("quoteResponse")]
        public QuoteResponse QuoteResponse { get; set; }
    }

    public class QuoteResponse
    {
        [JsonPropertyName("result")]
        public List<Result> Result { get; set; }

        [JsonPropertyName("error")]
        public object Error { get; set; }
    }

    public class Result
    {
        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("quoteType")]
        public string QuoteType { get; set; }

        [JsonPropertyName("typeDisp")]
        public string TypeDisp { get; set; }

        [JsonPropertyName("quoteSourceName")]
        public string QuoteSourceName { get; set; }

        [JsonPropertyName("triggerable")]
        public bool Triggerable { get; set; }

        [JsonPropertyName("customPriceAlertConfidence")]
        public string CustomPriceAlertConfidence { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("marketState")]
        public string MarketState { get; set; }

        [JsonPropertyName("regularMarketPrice")]
        public decimal RegularMarketPrice { get; set; }

        [JsonPropertyName("regularMarketDayHigh")]
        public decimal RegularMarketDayHigh { get; set; }

        [JsonPropertyName("regularMarketDayLow")]
        public decimal RegularMarketDayLow { get; set; }

        [JsonPropertyName("regularMarketOpen")]
        public decimal RegularMarketOpen { get; set; }

        [JsonPropertyName("regularMarketPreviousClose")]
        public decimal RegularMarketPreviousClose { get; set; }

        [JsonPropertyName("exchange")]
        public string Exchange { get; set; }

        [JsonPropertyName("exchangeTimezoneName")]
        public string ExchangeTimezoneName { get; set; }

        [JsonPropertyName("exchangeTimezoneShortName")]
        public string ExchangeTimezoneShortName { get; set; }

        [JsonPropertyName("gmtOffSetMilliseconds")]
        public long GmtOffSetMilliseconds { get; set; }

        [JsonPropertyName("market")]
        public string Market { get; set; }

        [JsonPropertyName("esgPopulated")]
        public bool EsgPopulated { get; set; }

        [JsonPropertyName("hasPrePostMarketData")]
        public bool HasPrePostMarketData { get; set; }

        [JsonPropertyName("firstTradeDateMilliseconds")]
        public long FirstTradeDateMilliseconds { get; set; }

        [JsonPropertyName("priceHint")]
        public int PriceHint { get; set; }

        [JsonPropertyName("regularMarketTime")]
        public long RegularMarketTime { get; set; }

        [JsonPropertyName("fullExchangeName")]
        public string FullExchangeName { get; set; }

        [JsonPropertyName("sourceInterval")]
        public int SourceInterval { get; set; }

        [JsonPropertyName("exchangeDataDelayedBy")]
        public int ExchangeDataDelayedBy { get; set; }

        [JsonPropertyName("tradeable")]
        public bool Tradeable { get; set; }

        [JsonPropertyName("cryptoTradeable")]
        public bool CryptoTradeable { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
    }
}
