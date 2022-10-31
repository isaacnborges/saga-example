using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Order.Domain.Interfaces;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class IndustryFailedEventConsumer : IConsumer<IndustryFailedEvent>
{
    private readonly ICartApiService _cartApiService;
    private readonly ILogger<IndustryFailedEventConsumer> _logger;

    public IndustryFailedEventConsumer(ICartApiService cartApiService, ILogger<IndustryFailedEventConsumer> logger)
    {
        _cartApiService = cartApiService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IndustryFailedEvent> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Integration with industry failed - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<IndustryFailedEvent>.ShortName);

        _logger.LogInformation("Rollback to cart api");

        var cartResponse = await _cartApiService.ReopenCart();
        _logger.LogInformation($"Carrinho reaberto - {cartResponse}");
    }
}