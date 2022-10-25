using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class OrderProcessedEventConsumer : IConsumer<OrderProcessedEvent>
{
    private readonly ILogger<OrderProcessedEventConsumer> _logger;

    public OrderProcessedEventConsumer(ILogger<OrderProcessedEventConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderProcessedEvent> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Order processed successfully - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentAuthorizedEvent>.ShortName);
    }
}
