using FinAlert.StockQueryService.Core.Contracts;
using FinAlert.StockQueryService.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinAlert.StockQueryService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStockQueryService(this IServiceCollection services)
    {
        services.AddHttpClient<YahooFinanceAuthenticator>();
        services.AddHttpClient<YahooQueryService>();
        services.AddSingleton<IYahooFinanceAuthenticator, YahooFinanceAuthenticator>();
        services.AddTransient<IStockQueryService, YahooQueryService>();

        return services;
    }
}