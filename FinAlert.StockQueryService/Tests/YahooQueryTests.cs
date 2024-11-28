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
        var logger = Mock.Of<ILogger<YahooQueryService>>();

        IYahooFinanceAuthenticator yahooAuth = new YahooFinanceAuthenticator(httpClient);

        var queryService = new YahooQueryService(httpClient, yahooAuth, logger);

        var price = await queryService.GetCurrentPriceAsync("AAPL");
    }
}