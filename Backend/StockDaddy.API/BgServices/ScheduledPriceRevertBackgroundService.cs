using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StockDaddy.Application.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace StockDaddy.API.Services
{
    public class ScheduledPriceRevertBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ScheduledPriceRevertBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(10); // Adjust as needed

        public ScheduledPriceRevertBackgroundService(IServiceProvider serviceProvider, ILogger<ScheduledPriceRevertBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var revertService = scope.ServiceProvider.GetRequiredService<ScheduledPriceRevertService>();
                        var productVariantService = scope.ServiceProvider.GetRequiredService<ProductVariantService>();
                        var now = DateTime.UtcNow;
                        Console.WriteLine(DateTime.UtcNow);
                        var dueReverts = await revertService.GetDueRevertsAsync(now);
                        foreach (var revert in dueReverts)
                        {
                            // Parse OriginalPricesJson: { "ProductVariantId": price, ... }
                            var priceMap = JsonSerializer.Deserialize<Dictionary<int, decimal>>(revert.OriginalPricesJson);
                            if (priceMap != null)
                            {
                                foreach (var kvp in priceMap)
                                {
                                    int variantId = kvp.Key;
                                    decimal originalPrice = kvp.Value;
                                    await productVariantService.RevertVariantPriceAsync(variantId, originalPrice);
                                }
                            }
                            await revertService.MarkAsCompletedAsync(revert.Id);
                            _logger.LogInformation("Reverted scheduled price for revertId={RevertId}", revert.Id);
                            _logger.LogInformation("Now: {Now}, RevertAt: {RevertAt}", now, revert.RevertAt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in ScheduledPriceRevertBackgroundService");
                }
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
