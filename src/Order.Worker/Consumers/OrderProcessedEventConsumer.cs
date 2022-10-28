using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
internal class OrderProcessedEventConsumer : IConsumer<OrderProcessedEvent>
{
    private readonly ILogger<OrderProcessedEventConsumer> _logger;

    public OrderProcessedEventConsumer(ILogger<OrderProcessedEventConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderProcessedEvent> context)
    {
        var timer = Stopwatch.StartNew();

        await Task.Delay(TimeSpan.FromSeconds(6));

        _logger.LogInformation($"Order processed successfully - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentAuthorizedEvent>.ShortName);
    }
}
