using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
internal class OrderProcessedEvent2Consumer : IConsumer<OrderProcessedEvent2>
{
    private readonly ILogger<OrderProcessedEvent2Consumer> _logger;

    public OrderProcessedEvent2Consumer(ILogger<OrderProcessedEvent2Consumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderProcessedEvent2> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Order processed successfully - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentAuthorizedEvent2>.ShortName);
    }
}
