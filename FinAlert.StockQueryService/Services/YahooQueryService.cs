using System.Text.Json;
using FinAlert.StockQueryService.Core.Contracts;
using FinAlert.StockQueryService.Core.Domain;
using FinAlert.StockQueryService.Core.Models;
using Microsoft.Extensions.Logging;

namespace FinAlert.StockQueryService.Services;

/*
    * Obtain crump: https://query2.finance.yahoo.com/v1/test/getcrumb / query1
    * Repsol chart: https://query1.finance.yahoo.com/v8/finance/chart/rep.mc
    * Repsol quote: https://query1.finance.yahoo.com/v7/finance/quote?symbols=REP.MC&crumb=YOURCRUMB
*/

public class YahooQueryService : IStockQueryService
{
    private readonly IYahooFinanceAuthenticator _authenticator;
    private readonly ILogger<YahooQueryService> _logger;
    private readonly HttpClient _httpClient;

    public YahooQueryService(HttpClient httpClient,
        IYahooFinanceAuthenticator yahooAuthenticator,
        ILogger<YahooQueryService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _authenticator = yahooAuthenticator;
    }

    public async Task<StockPrice> GetCurrentPriceAsync(string symbol)
    {
        await _authenticator.EnsureAuthenticationAsync();

        string requestFields =
            "regularMarketPreviousClose,regularMarketOpen,currency,regularMarketTime,regularMarketDayLow,regularMarketDayHigh";

        var requestUri = new UriBuilder("https://query1.finance.yahoo.com/v7/finance/quote")
        {
            Query = $"symbols={symbol}&crumb={_authenticator.GetCrumb()}&fields={requestFields}"
        }.ToString();

        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        request.Headers.UserAgent.ParseAdd(YahooFinanceAuthenticator.USER_AGENT);
        request.Headers.Add("Cookie", $"{_authenticator.GetAuthCookie().Name}={_authenticator.GetAuthCookie().Value}");

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var yahooResponse = JsonSerializer.Deserialize<QuoteRoot>(json);

        var result = yahooResponse?.QuoteResponse?.Result?.FirstOrDefault();
        if (result == null)
            return null!;

        return new StockPrice
        {
            Currency = result.Currency,
            Current = result.RegularMarketPrice,
            DayHigh = result.RegularMarketDayHigh,
            DayLow = result.RegularMarketDayLow,
            Open = result.RegularMarketOpen,
            PreviousClose = result.RegularMarketPreviousClose,
            MarketTime = DateTimeOffset.FromUnixTimeSeconds(result.RegularMarketTime).UtcDateTime
        };
    }
}