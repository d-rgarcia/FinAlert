using FinAlert.StockQueryService.Core.Domain;

namespace FinAlert.StockQueryService.Core.Contracts;

public interface IStockQueryService
{
    Task<StockPrice> GetCurrentPriceAsync(string symbol);
}