using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class OrderCreatedEvent2Consumer : IConsumer<OrderCreatedEvent2>
{
    private readonly ILogger<OrderCreatedEvent2Consumer> _logger;
    private readonly IBus _bus;

    public OrderCreatedEvent2Consumer(ILogger<OrderCreatedEvent2Consumer> logger, IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent2> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Order processed successfully - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<OrderCreatedEvent2>.ShortName);

        var command = new AuthorizePaymentCommand2(context.Message.OrderId, context.Message.CustomerName, InVar.Timestamp);
        await _bus.Publish(command);
        _logger.LogInformation($"Send AuthorizePaymentCommand2 - OrderId: {command.OrderId}");
    }
}
