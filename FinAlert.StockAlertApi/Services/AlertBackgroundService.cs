using System.Diagnostics;
using FinAlert.AlertStore.Core.Contracts;
using FinAlert.AlertStore.Core.Domain;
using FinAlert.StockAlertApi.Options;
using FinAlert.StockQueryService.Core.Contracts;
using FinAlert.StockQueryService.Core.Domain;
using Microsoft.Extensions.Options;

public class AlertBackgroundService : BackgroundService
{
    private readonly IStockQueryService _stockQueryService;
    private readonly ILogger<AlertBackgroundService> _logger;
    private readonly IOptions<AlertMonitorOptions> _options;
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<string, StockPrice> cachedStockPrices = new();

    public AlertBackgroundService(IStockQueryService stockQueryService, IOptions<AlertMonitorOptions> options,
        IServiceProvider serviceProvider, ILogger<AlertBackgroundService> logger)
    {
        _logger = logger;
        _stockQueryService = stockQueryService;
        _options = options;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateAsyncScope();

            var alertStoreService = scope.ServiceProvider.GetRequiredService<IPriceAlertService>();

            var activeAlerts = (await alertStoreService.GetEnabledAlertsAsync()).ToList();

            foreach (var alert in activeAlerts)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                try
                {
                    StockPrice stockPrice = cachedStockPrices.GetValueOrDefault(alert.Symbol)
                        ?? await _stockQueryService.GetCurrentPriceAsync(alert.Symbol);
                

                    if (stockPrice is null)
                    {
                        _logger.LogWarning($"Price for {alert.Symbol} not found, skipping alert.");
                        continue;
                    }

                    if (isAlertTriggered(alert, stockPrice))
                        await alertStoreService.TriggerAlertAsync(alert.UserId, alert.Id);

                    cachedStockPrices.TryAdd(alert.Symbol, stockPrice);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing alert for {Symbol}.", alert.Symbol);
                }
            }

            cachedStockPrices.Clear();

            await Task.Delay(TimeSpan.FromSeconds(_options.Value.ExecutionIntervalSeconds), stoppingToken);
        }
    }

    private bool isAlertTriggered(PriceAlert alert, StockPrice stockPrice)
    {
        if (alert.AlertType == PriceAlertType.Price)
        {
            return alert.TriggerType switch
            {
                PriceTriggerType.Above => stockPrice.DayHigh >= alert.Threshold,
                PriceTriggerType.Below => stockPrice.DayLow <= alert.Threshold,
                _ => true
            };
        }
        else
        {
            return alert.TriggerType switch
            {
                PriceTriggerType.Above => (stockPrice.DayHigh - stockPrice.Open) / stockPrice.Open >= alert.Threshold,
                PriceTriggerType.Below => (stockPrice.DayLow - stockPrice.Open) / stockPrice.Open <= alert.Threshold,
                PriceTriggerType.Both => (stockPrice.DayHigh - stockPrice.Open) / stockPrice.Open >= alert.Threshold
                                            || (stockPrice.DayLow - stockPrice.Open) / stockPrice.Open <= alert.Threshold,
                _ => false
            };
        }
    }
}