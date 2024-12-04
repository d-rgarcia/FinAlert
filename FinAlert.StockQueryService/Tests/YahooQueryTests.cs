using FinAlert.StockQueryService.Core.Contracts;
using FinAlert.StockQueryService.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FinAlert.StockQueryService.Tests;

public class YahooQueryTests
{
    [Fact]
    public async Task YahooQueryService_GetStockPrice_ReturnsPrice()
    {
        var httpClient = new HttpClient();

        IYahooFinanceAuthenticator yahooAuth = new YahooFinanceAuthenticator(httpClient, Mock.Of<ILogger<YahooFinanceAuthenticator>>());

        var queryService = new YahooQueryService(httpClient, yahooAuth, Mock.Of<ILogger<YahooQueryService>>());

        var price = await queryService.GetCurrentPriceAsync("AAPL");
    }
}